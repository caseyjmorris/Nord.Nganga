var gulp = require('gulp');
var uglify = require('gulp-uglify');
var templateCache = require('gulp-angular-templatecache');
var concat = require('gulp-concat');
var rename = require('gulp-rename');
var sourcemaps = require('gulp-sourcemaps');
var cleancss = require('gulp-clean-css');
var merge = require('merge-stream');

gulp.task('default', function() {
  // place code for your default task here
});

var BUILD = 'build';

gulp.task('minify-css', function()
{
  return gulp.src('css/**/*.css')
    .pipe(concat('nganga.dist.css'))
    .pipe(cleancss({compatibility: 'ie8'}))
    .pipe(gulp.dest(BUILD));
})

gulp.task('minify', function()
{
  var templates =   gulp.src('js/templates/**/*.html')
    .pipe(templateCache({
      transformUrl : function(url){return '/client/lib/ng/plugin/nganga/js/templates/' + url;},
      module: 'nganga.ui'
    }));
    
  var srcCode = gulp.src(['js/nganga.js', 'js/interpolate.js', 'js/http.handling.js', 'js/exception.handling.js', 'js/svc/**/*.js', 'js/ui/**/*.js'])
  .pipe(sourcemaps.init());
  
  var stream = merge(templates, srcCode);
  
  stream
  .pipe(concat('nganga.dist.js'))
  .pipe(gulp.dest(BUILD))
  .pipe(rename('nganga.dist.min.js'))
  .pipe(uglify())
  .pipe(sourcemaps.write('./'))
  .pipe(gulp.dest(BUILD))
});