var Locale = (function (re, $) {
    "use strict";
    var __userLocale = '';
    var __sysLocale = __kachuwaSettings.LocaleRegion.Culture;
    var _locales = [];
    var _userlocales = [];
    var getLocale = function (key,group) {
        var selected=null;
        if (group == undefined || group == "") {
            var i = 0;
            for (i = 0; i < _locales.length; i++) {
             
                var group = _locales[i];
                var found = false;
                var j = 0;
                for (j = 0; j < group.Locales.length; j++) {
                   
                    if (group.Locales[j].Name == key) {
                        selected = group.Locales[j];
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }
            if (selected == null)
                return key;
            return selected.Value;
        }
        else {
            var found = false;
            var i = 0;
            for (i = 0; i < _locales.length; i++) {

                if (_locales[i].Group == group) {
                    var group = _locales[i];
                    var j = 0;
                    for (j = 0; j < group.Locales.length; j++) {
                        if (group.Locales[j].Name == key) {
                            selected = group.Locales[j];
                            found = true;
                            break;
                        }
                    }

                } 
                if (found)
                    break;
               
            }
            if (!found) {
                var i = 0;
                for (i = 0; i < _locales.length; i++) {

                    var group = _locales[i];
                    var found = false;
                    var j = 0;
                    for (j = 0; j < group.Locales.length; j++) {
                        if (group.Locales[j].Name == key) {
                            selected = group.Locales[j];
                            found = true;
                            break;
                        }
                    }
                    if (found)
                        break;
                }
                
            }
            if (selected == null)
                return key;
            return selected.Value;
        }
       
    };
    var loadJson = function(culture) {
        $.getJSON("/locale/locale_resources-" +culture+".json", function (resources) {
          
        });
    }
    var init = function() {
        if (__userLocale == "" && __sysLocale != "") {
            $.getJSON("/locale/locale_resources-" + __sysLocale + ".json", function (resources) {
                _locales = resources;
            });
        }
        if (__userLocale != "") {
            $.getJSON("/locale/locale_resources-" + __userLocale + ".json", function (resources) {
                _userlocales = resources;
            });
        }
    }
    var update = function(userLocalCulture) {
        $.getJSON("/locale/locale_resources-" + userLocalCulture + ".json", function (resources) {
            _userlocales = resources;
        });
    }
    init();
    return { Get: getLocale, UpdateUserLocale: update };

}(Locale || {}, jQuery))