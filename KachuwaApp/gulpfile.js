/// <binding AfterBuild='default' />
var gulp = require('gulp');
var sass = require('gulp-sass');
var sourcemaps = require('gulp-sourcemaps');
var bs = require('browser-sync').create();
var rename = require('gulp-rename');

function compile_default() {
    return gulp.src('./themes/shared/default/scss/theme.scss')
        .pipe(sourcemaps.init())
        .pipe(sass().on('error', sass.logError))
        .pipe(sourcemaps.write('./maps'))
        .pipe(gulp.dest('./themes/shared/default/css'))
        .pipe(bs.reload({
            stream: true
        }));
}

gulp.task('compile_default', compile_default);
gulp.task('compile_default_prod', function () {
    return gulp.src('./themes/shared/default/scss/theme.scss')
        .pipe(sass({ outputStyle: 'compressed' }).on('error', sass.logError))
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest('./themes/shared/default/css'))
        .pipe(bs.reload({
            stream: true
        }));
});


gulp.task('compile_admin', function () {
    return gulp.src('./themes/shared/admin/scss/theme.scss')
        .pipe(sourcemaps.init())
        .pipe(sass().on('error', sass.logError))
        .pipe(sourcemaps.write('.'))
        .pipe(gulp.dest('./themes/shared/admin/css'))
        .pipe(bs.reload({
            stream: true
        }));
});
gulp.task('compile_admin_prod', function () {
    return gulp.src('./themes/shared/admin/scss/theme.scss')
        .pipe(sass({ outputStyle: 'compressed' }).on('error', sass.logError))
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest('./themes/shared/admin/css'))
        .pipe(bs.reload({
            stream: true
        }));
});

//Watch task
gulp.task('watch_admin_theme', function () {
    gulp.watch('./themes/shared/admin/**/*.scss', gulp.series('compile_admin', 'compile_admin_prod'));
});

//Watch task
gulp.task('watch_default_theme', function () {
    gulp.watch('./themes/shared/default/**/*.scss', gulp.series('compile_default', 'compile_default_prod'));
});


gulp.task('watch', gulp.series('watch_admin_theme', 'watch_default_theme'));

gulp.task('default', gulp.parallel( 'compile_default', 'compile_default_prod',  'compile_admin',
    'compile_admin_prod'
   )); //, ]);

gulp.task('dev', gulp.parallel( 'compile_default', 'compile_admin',  'watch')); //, ']);