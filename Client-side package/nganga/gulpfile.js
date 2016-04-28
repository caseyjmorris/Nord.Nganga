var gulp = require('gulp');
var uglify = require('gulp-uglify');
var templateCache = require('gulp-angular-templatecache');
var concat = require('gulp-concat');
var rename = require('gulp-rename');
var sourcemaps = require('gulp-sourcemaps');

gulp.task('default', function() {
  // place code for your default task here
});

var BUILD = 'build';

gulp.task('minify', function()
{
  gulp.src(['js/nganga.js', 'js/interpolate.js', 'js/http.handling.js', 'js/exception.handling.js', 'js/svc/**/*.js', 'js/ui/**/*.js'])
  .pipe(sourcemaps.init())
  .pipe(concat('nganga.dist.js'))
  .pipe(gulp.dest(BUILD))
  .pipe(rename('nganga.dist.min.js'))
  .pipe(uglify())
  .pipe(sourcemaps.write('./'))
  .pipe(gulp.dest(BUILD))
});

gulp.task('processTemplates', function()
{
  gulp.src('js/templates/**/*.html')
    .pipe(templateCache({
      transformUrl : function(url){return '/client/lib/ng/plugin/nganga/js/templates/' + url;},
      module: 'nganga.ui'
    }))
    .pipe(gulp.dest(BUILD))
});