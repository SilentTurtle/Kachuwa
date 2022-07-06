const { watch, series, src, dest } = require("gulp");
var browserSync = require("browser-sync").create();
var postcss = require("gulp-postcss");
var sass = require('gulp-sass')(require('sass'));
var sourcemaps = require('gulp-sourcemaps');
// Task for compiling our CSS files using PostCSS
function cssTask(cb) {
	return src("./themes/shared/default/scss/main.scss") // read .css files from ./src/ folder
		.pipe(sourcemaps.init({ loadMaps: true }))
		.pipe(sass()) // compile using postcss

		.pipe(sourcemaps.write('./map'))
		.pipe(dest("./themes/shared/default/css")) // paste them in ./assets/css folder
		.pipe(browserSync.stream());
	cb();
}
function admin(cb) {
	return src("./themes/shared/admin/scss/theme.scss") // read .css files from ./src/ folder
		.pipe(sourcemaps.init({ loadMaps: true }))
		.pipe(sass()) // compile using postcss

		.pipe(sourcemaps.write('./map'))
		.pipe(dest("./themes/shared/admin/css")) // paste them in ./assets/css folder
		.pipe(browserSync.stream());
	admin();
}

function browsersyncServe(cb) {
	browserSync.init({
		server: {
			baseDir: "./",
		},
	});
	cb();
}

function browsersyncReload(cb) {
	browserSync.reload();
	cb();
}

// Watch Files & Reload browser after tasks
function watchTask() {
	//watch("./**/*.html", browsersyncReload);
	watch(["./themes/shared/default/scss/*.scss"], series(cssTask, browsersyncReload));
	watch(["./themes/shared/admin/scss/*.scss"], series(admin, browsersyncReload));
}

// Default Gulp Task
exports.default = series( admin, browsersyncServe, watchTask);
exports.css = series(admin,cssTask);
exports.watch = series(watchTask);
