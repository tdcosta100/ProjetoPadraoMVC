﻿/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var del = require('del');
var rename = require('gulp-rename');
var util = require('gulp-util');

var paths = {
    packageRoot: "./bower_components/",
    projectRoot: "./"
};

var packages = {
    "bootstrap": [
        {
            src: ["dist/css/*.css", "dist/css/*.map"],
            base: "dist/css/",
            dest: "Content/CSS/"
        },
        {
            src: ["dist/fonts/glyphicons-halflings-regular.*"],
            base: "dist/fonts/",
            dest: "Content/Fonts/"
        },
        {
            src: ["dist/js/*.js", "!dist/js/npm.js"],
            base: "dist/js/",
            dest: "Scripts/"
        }
    ],
    "jquery": [
        {
            src: ["dist/*.js", "dist/*.map"],
            base: "dist/",
            dest: "Scripts/"
        }
    ],
    "jquery-validation": [
        {
            src: ["dist/*.js"],
            base: "dist/",
            dest: "Scripts/"
        }
    ],
    "jquery-validation-unobtrusive": [
        {
            src: ["*.js"],
            base: "",
            dest: "Scripts/"
        }
    ],
    "modernizr": [
        {
            src: ["modernizr.js"],
            base: "",
            dest: "Scripts/"
        }
    ],
    "respond": [
        {
            src: ["dest/*.js"],
            base: "dest/",
            dest: "Scripts/"
        }
    ],
    "ckeditor": [
        {
            src: ["**/*"],
            base: "",
            dest: "Scripts/ckeditor/"
        }
    ],
    "ace-builds": [
        {
            src: ["src-min-noconflict/**/*"],
            base: "src-min-noconflict/",
            dest: "Scripts/ace/"
        }
    ],
    "jstree": [
        {
            src: ["dist/themes/**"],
            base: "dist/themes/",
            dest: "Content/CSS/jstree-themes/"
        },
        {
            src: ["dist/*.js"],
            base: "dist/",
            dest: "Scripts/"
        }
    ],
    "moment": [
        {
            src: ["min/moment.min.js"],
            base: "min/",
            dest: "Scripts/"
        },
        {
            src: ["locale/pt-br.js"],
            base: "locale/",
            rename: "moment.locale.pt-br.js",
            dest: "Scripts/"
        }
    ],
    "eonasdan-bootstrap-datetimepicker": [
        {
            src: ["build/css/**"],
            base: "build/css/",
            dest: "Content/CSS/"
        },
        {
            src: ["build/js/*.js"],
            base: "build/js/",
            dest: "Scripts/"
        }
    ]
};

gulp.task('clean-packages', function()
{
    del([
        'Content/CSS/**',
        '!Content/CSS',
        '!Content/CSS/Site.css',
        'Content/Fonts/**',
        '!Content/Fonts',
        'Scripts/**',
        '!Scripts',
        '!Scripts/Site.js',
        '!Scripts/ace',
		'!Scripts/ace/theme-visualstudio.js'
    ],
        { cwd: paths.projectRoot }
    )
    .then(paths =>
    {
        console.log('Deleted:\n', paths.join('\n'));
    });
});

gulp.task('copy-packages', function ()
{
    for (var packageName in packages) {
        for (var i in packages[packageName]) {
            var packageItem = packages[packageName][i];

            console.log("Processing package " + packageName + "...");

            gulp.src(packageItem.src, { cwd: paths.packageRoot + "/" + packageName, base: paths.packageRoot + "/" + packageName + "/" + packageItem.base })
            .pipe((typeof(packageItem.rename) !== "undefined") ? rename(packageItem.rename) : util.noop())
            .pipe(gulp.dest(packageItem.dest, { cwd: paths.projectRoot }));

            console.log("Done!\n");
        }
    }
});

gulp.task('default', function () {
    // place code for your default task here
});