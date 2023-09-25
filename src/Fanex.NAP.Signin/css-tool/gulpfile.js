var gulp = require('gulp');
var base64 = require('gulp-base64-inline');
gulp.task('default', function() {
  return gulp
    .src('src/login.css')
    .pipe(base64(''))
    .pipe(gulp.dest('dist'));
});
