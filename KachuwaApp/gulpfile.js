var gulp = require('gulp');
var sass = require('gulp-sass');

gulp.task('sass', function () {
    console.log(sass);
    return gulp.src('./themes/shared/default/assets/sass/theme.scss')
        .pipe(sass().on('error', sass.logError))
        .pipe(gulp.dest('./themes/shared/default/assets/css'));
});

gulp.task('sass:watch', function () {
    console.log("starting watching...");
    gulp.watch('./themes/shared/default/assets/sass/framework_components/*.scss', './themes/shared/default/assets/sass/theme_components/*.scss', ['sass']);
});
