(function()
  {
    // This repeatedly uses the += operator to build up strings... according to what I've read, in most browsers
    // (i.e., not IE < 7), that is actually as fast or faster than using an array and calling join, even though it
    // certainly looks wrong.  c.f. http://www.sitepoint.com/javascript-fast-string-concatenation/

    function provider()
      {
        var obj;

        var tokens = {
          START_INTERPOLATION: '<',
          STOP_INTERPOLATION: '>',
          PROPERTY_SEPARATOR: '.',
          ESCAPE_NEXT: '\\'
        };

        function isToken(input)
          {
            for (var key in tokens)
              {
                if(input === tokens[key])
                  {
                    return true;
                  }
              }

            return false;
          }

        var propStart = /[A-Za-z]/;

        var propBody = /[A-Za-z0-9\.]/;

        var targets = [];

        var currentStBody = '';

        var next;

        var prev;

        var currentPropRefBody = '';

        var templateBody;

        var interpolationContext = {
          normal: 0,
          propReference: 1,
          escaped: 2
        };

        var currentContext = interpolationContext.normal;

        function closeStBody()
          {
            targets.push({
              text: currentStBody,
              context: interpolationContext.normal
            });
            currentStBody = '';
          }

        function closePropRefBody()
          {
            targets.push({
              text: currentPropRefBody,
              context: interpolationContext.propReference
            });
            currentPropRefBody = '';
          }

        function processNextCharacterNormal()
          {
            if (next === tokens.ESCAPE_NEXT)
              {
                currentContext = interpolationContext.escaped;
                return;
              }

            if (prev === tokens.START_INTERPOLATION && next === tokens.START_INTERPOLATION)
              {
                currentContext = interpolationContext.propReference;
                closeStBody();
              }
            else if (prev !== tokens.START_INTERPOLATION && next !== tokens.START_INTERPOLATION)
              {
                currentStBody += next;
              }
            else if(prev === tokens.START_INTERPOLATION && next !== tokens.STOP_INTERPOLATION)
              {
                currentStBody += tokens.START_INTERPOLATION;
                currentStBody += next;
              }
          }

        function processNextCharacterEscaped()
          {
            currentStBody += next;
            next = '';
            currentContext = interpolationContext.normal;
          }

        function processNextCharacterPropRef()
          {
            if ((!isToken(prev) && !isToken(next)) || (prev === tokens.START_INTERPOLATION && !isToken(next)))
              {
                var isValid = currentPropRefBody.length === 0
                  ? propStart.test(next)
                  : propBody.test(next);

                if (!isValid)
                  {
                    throw {error: 'Invalid prop ref'};
                  }
                currentPropRefBody += next;
                return;
              }

            if (prev === tokens.STOP_INTERPOLATION && next === tokens.STOP_INTERPOLATION)
              {
                currentContext = interpolationContext.normal;
                closePropRefBody();
              }
            else if (isToken(prev) && isToken(next) && prev !== next)
              {
                throw {error: 'Invalid prop ref'};
              }
            else if(prev === tokens.STOP_INTERPOLATION && next !== tokens.STOP_INTERPOLATION)
              {
                throw {error: 'Invalid prop ref'}
              }
            else if (isToken(prev) && next === tokens.PROPERTY_SEPARATOR)
              {
                throw {error:  'Invalid prop ref'}
              }
            else if (prev !== tokens.PROPERTY_SEPARATOR && next === tokens.PROPERTY_SEPARATOR)
              {
                currentPropRefBody += tokens.PROPERTY_SEPARATOR;
              }
            else if (prev === tokens.PROPERTY_SEPARATOR && next !== tokens.PROPERTY_SEPARATOR)
              {
                currentPropRefBody += next;
              }
          }

        function resolveProperty(src, str, depth)
          {
            depth = depth || 0;

            var propPath = str.split(tokens.PROPERTY_SEPARATOR);

            var prop = propPath[depth];

            if (!src.hasOwnProperty(prop))
              {
                throw {error:  'Object does not define field', src: src, propertyReference: str};
              }

            if (++depth == propPath.length)
              {
                return src[prop];
              }
            else
              {
                return resolveProperty(src[prop], str, depth);
              }
          }

        function joinTargets()
          {
            var final = '';
            for (var i = 0; i < targets.length; i++)
              {
                var cur = targets[i];
                if (cur.context === interpolationContext.normal)
                  {
                    final += cur.text;
                  }
                else if (cur.context === interpolationContext.propReference)
                  {
                    try
                      {
                        final += resolveProperty(obj, cur.text);
                      }
                    catch(err)
                      {
                        err.untilError = final;
                        err.template = templateBody;
                        throw err;
                      }
                  }
              }

            return final;
          }

        function interpolate(input, propertiesObject)
          {
            templateBody = input;

            obj = propertiesObject;
            for (var i = 0; i < input.length ; i++)
              {
                next = input.charAt(i);

                if (currentContext === interpolationContext.normal)
                  {
                    processNextCharacterNormal();
                  }
                else if (currentContext === interpolationContext.propReference)
                  {
                    processNextCharacterPropRef();
                  }
                else if (currentContext === interpolationContext.escaped)
                  {
                    processNextCharacterEscaped();
                  }

                prev = next;
              }

            if (currentPropRefBody.length > 0)
              {
                throw {error: 'Unterminated property reference in interpolation source',
                  source: input,
                  propRefBody: currentPropRefBody};
              }

            if (currentContext === interpolationContext.escaped)
              {
                throw {error:  'Unterminated escape sequence',
                source: input};
              }

            if (currentContext === interpolationContext.normal)
              {
                closeStBody();
              }

            return joinTargets();
          }

        return interpolate;
      }

    window.nganga = window.nganga || {};
    window.nganga.interpolate = function(input, propertiesObject)
      {
        return provider()(input, propertiesObject);
      }
  })();