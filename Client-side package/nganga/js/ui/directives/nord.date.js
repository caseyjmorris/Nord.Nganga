angular.module('nganga.ui').directive('nordDate', [
  function () {

    // a custom date directive intended to normalize date behavior on browsers not 
    // supporting built-in date types 
    // 
    // notes: 
    //      both isLeapYear and daysInMonth exploit the Date object ability to "roll over" 
    //      dates based on months or days exceeding 'normal' ranges... 
    //
    // i.e. 
    //      new Date(2000,0,32) => Tue Feb 01 2000
    //      new Date(2001,0,367) => Wed Jan 02 2002
    //      new Date(2001,13,368) => Mon Feb 03 2003 
    //
    // remember that the month argument of the Date constructor is ZERO based - idiotic!
    //

    // lookup data 
    var monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    var dayConstants = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

    // return a string representation of an integer padded with leading zeros 
    var pad = function (value, size) {
      return ( 1e15 + value + "" ).slice(-size); 
    }


    // returns a boolean TRUE if the specified input year is a leap year 
    // used by days in month to determine days for february 
    var isLeapYear = function (year) {
      return (366 - new Date(year, 0, 366).getDate()) !== 365;
    };
    

    // determine how many days are in a given month
    // honors leap years and returns the correct value for any given month/year pair
    var daysInMonth = function (iMonth, iYear) {
      var constant = dayConstants[iMonth-1] + 1 ;
      if (iMonth === 2) {
        if (isLeapYear(iYear)) {
          constant = constant + 1;
        }
      }
      return constant - new Date(iYear, iMonth-1, constant).getDate();
    };
    
    // attempt to tokenize a string in the format:
    //    <month-integer>/<day-integer>/<year-integer> 
    //      or 
    //    <month-integer>-<day-integer>-<year-integer>
    // 
    // this routine does not validate numeric ranges, it only tokenizes 
    // returns an object with the date parts as integer properties
    // accepts either forward slash or dash characters as separators 
    var parseDateElements = function (strVal) {

      // give priority to the forward slash...
      var s1 = strVal.split("/");
      if (s1.length === 3) {
        return {
          month: parseInt(s1[0], 10),
          day: parseInt(s1[1], 10),
          year: parseInt(s1[2], 10)
        };
      }

      // if we're still here then we found no valid 
      // forward slash delimited date... 
      // so check for a dash delimited date 
      var s2 = strVal.split("-");
      if (s2.length === 3) {
        return {
          month: parseInt(s2[0], 10),
          day: parseInt(s2[1], 10),
          year: parseInt(s2[2], 10)
        };
      }
      return undefined;
    };

    // attempt to convert a string in the format MM/DD/YY[YY] or MM-DD-YY[YY] into a date object 
    // returns undefined for invalid input 
    var parseDate = function (strVal) {
      // parse out the parts of the date string 
      var elements = parseDateElements(strVal);

      // if the parse failed then we don't have enough/quality data to get a date 
      if (elements === undefined || elements === null) return undefined;

      // if any part of the date did not parese into numbers then we cant continue 
      if (isNaN(elements.year) || isNaN(elements.month) || isNaN(elements.day)) return undefined;

      // ensure the month is valid 
      if (elements.month < 1 || elements.month > 12) return undefined;

      // make sure the year is appropriate
      // three digit years are not valid
      if (elements.year > 99 && elements.year < 1900) return undefined; 

      // one or two digit years are assumed to be 19.. 
      if (elements.year < 100) { 
        elements.year = elements.year + 1900;
      }

      // make sure the day is valid based on the current month 
      var maxDays = daysInMonth(elements.month, elements.year);
      if (elements.day < 1 || elements.day > maxDays) return undefined;

      // we use the 'named month' date string model to ensure that the parse NEVER confuses a month and day based on machine or locale settings 
      var dateString = pad(elements.day, 2) + ' ' + monthNames[elements.month - 1] + ' ' + pad(elements.year, 4);

      // parse the date string into a timestamp
      var timestamp = Date.parse(dateString);

      // if the parse failed then we cannot convert to a date 
      if (isNaN(timestamp)) return undefined;

      // convert the timestamp to a date object 
      var date = new Date(timestamp);

      // we're good ! 
      return date;
    }

    // determine if the current browser intrinsicly supports date types
    // this is known to return false for IE (up to 11.0.9600) and true for Chrome 
    var isDateSupported = function () {
      var input = document.createElement('input');
      input.setAttribute('type', 'date');

      var notADateValue = 'not-a-date';
      input.setAttribute('value', notADateValue);

      return !(input.value === notADateValue);
    };

    var endsWith = function(str, suffix) {
      return str.indexOf(suffix, str.length - suffix.length) !== -1;
    };

    // this function allows the specification of a user format provider as the attribute 'nord-formatter'
    //
    // valid formats of this specfifier are:
    //    myFormatter             - this function must exist ON the date object instance => dateInstanceFromModel.myFormatter()
    //    myFormatter()           - this function must exist ON the date object instance => dateInstanceFromModel.myFormatter() 
    //    myFormatter($value)     - this function must exist ON the $scope object and the $value will be replaced by the date object instance => $scope.myFormatter(dateInstanceFromModel) 
    //
    var invokeFormatter = function ($scope, inputValue, formatterDeclaration) {
      
      //if (!formatterDeclaration) {
      //  return inputValue;
      //}

      var decl = formatterDeclaration || 'toLocaleDateString()';

      if (!endsWith(decl, ')')) { // no parens - i.e. no argument so function is assumed to be on  the value
        return inputValue[decl]();
      }

      if (endsWith(decl, '()')) { // empty parens - i.e. no argument so function is assumed to be on  the value
        decl = decl.substring(0, decl.length - 2);
        return inputValue[decl]();
      }

      if (endsWith(decl, '($value)')) { // function takes an argument so function is assumed to be in scope and the $value will be replaced with the input
        var op = decl.indexOf('(');
        decl = decl.substring(0,op);
        
        return $scope[decl](inputValue);
      }

      return inputValue; // cannot result this mess.... 

    };

    // if dates are supported by the browser 
    if (isDateSupported()) {
      // then use a simple template exploting the input element with type ="date" 
      // no special processing is required in this model
      return {
        restrict: 'E',
        templateUrl: window.ngangaTemplateLocation + 'nord.date.intrinsic.html',
        replace: true
      }
    } else {
      // we need to parse values coming from the interface 
      // and format values headed out to the interface 
      // no special processing is required in this model
      return {
        restrict: 'E',
        templateUrl: window.ngangaTemplateLocation + 'nord.date.shim.html',
        replace: true,
        require: 'ngModel',
        link: function ($scope, $element, $attrs, modelCtrl) {
          // we register two handlers 
          // one for each direction of the model binding 

          // the formatters are invoked when the model changes
          // and are used to alter the displayed value 
          modelCtrl.$formatters.push(function (inputValue) {
            // this inputValue is coming from the MODEL object 
            if (inputValue) {
              if (angular.isDate(inputValue)) {
                return invokeFormatter($scope, inputValue, $attrs.nordFormatter);
              }
            }
            // this return value is headed to the UI
            return inputValue;
          });

          // the parsers are invoked with the interface value changes 
          // and are used to validate and/or parse the string value 
          // into strongly typed objects for the model
          // if the parse fails, the date field is flagged as invalid
          modelCtrl.$parsers.push(function (inputValue) {
            // this inputValue is coming from the UI 
            var returnValue = parseDate(inputValue);
            var blank = inputValue === '' || inputValue === null || inputValue === undefined;
            var isValid = returnValue !== undefined || blank;
            modelCtrl.$setValidity('nordDate', isValid);
            // this return value is headed to the MODEL object
            if (blank)
              {
                return '';
              }
            return returnValue;
          });
        }
      }
    }
  }
]);
