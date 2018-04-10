var gulp = require('gulp');
var sass = require('gulp-sass');
var sourcemaps = require('gulp-sourcemaps');
var bs = require('browser-sync').create();

gulp.task('sass', function () {
    return gulp.src('./themes/shared/default/sass/theme.scss')
        .pipe(sourcemaps.init())
        .pipe(sass())
        .pipe(sourcemaps.write('.'))
        .pipe(gulp.dest('./themes/shared/default/css'))
        .pipe(bs.reload({
            stream: true
        }));
});
 
//Watch task
gulp.task('default',['sass','frontendwtc']);


gulp.task('compileadmin', function () {
    return gulp.src('./themes/shared/admin/sass/theme.scss')
        .pipe(sourcemaps.init())
        .pipe(sass())
        .pipe(sourcemaps.write('.'))
        .pipe(gulp.dest('./themes/shared/admin/css'))
        .pipe(bs.reload({
            stream: true
        }));
});
//Watch task
gulp.task('adminwtc',function() {
    gulp.watch('./themes/shared/admin/**/*.scss',['compileadmin']);
});
 
//Watch task
gulp.task('frontendwtc',function() {
    gulp.watch('./themes/shared/default/**/*.scss',['sass']);
});



 