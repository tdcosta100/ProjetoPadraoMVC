

( function(){

CKEDITOR.plugins.add( 'imageuploader', {
   	 init: function( editor ) {
		sessionStorage.setItem('editor', editor);
		editor.config.filebrowserBrowseUrl = "/resources/ckeditor/custom/plugins/imageuploader/upload_img.html";
	},

	afterInit: function(){
	//	$(this).closest('.cke_dialog cke_browser_webkit cke_hidpi cke_ltr').css('visibility','hidden');
		var ed = sessionStorage.getItem('editor');
		var href = sessionStorage.getItem('href');
		//insertImage1(ed, href);

		function insertImage1(editor, href) {
        	   		 var elem = editor.document.createElement('img', {
         	    		   attributes: {
        	           		 src: href
                		   }
                               });
            		       editor.insertElement(elem);
                	}
	}
});


CKEDITOR.plugins.image = {
			insertImage: function(editor, href) {
        	   		 var elem = editor.document.createElement('img', {
         	    		   attributes: {
        	           		 src: href
                		   }
                               });
            		       editor.insertElement(elem);
                	}

              };
})();
