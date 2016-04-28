var gulp = require('gulp');
var uglify = require('gulp-uglify');
var templateCache = require('gulp-angular-templatecache');

gulp.task('default', function() {
  // place code for your default task here
});

var BUILD = 'build';

gulp.task('minify', function()
{
  gulp.src('js/**/*.js')
  .pipe(uglify())
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