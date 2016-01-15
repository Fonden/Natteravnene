// ================================================================
//  Description: File Upload supporting script
//  License:     MIT - check License.txt file for details
//  Author:      Natteravnene - Dan Taxbøl
// ================================================================
$(function () {
    if (typeof $('#file-upload-form') !== undefined) {
        var maxSizeAllowed = 4; // ToDo - Change upload limit in MB

        initFileUpload();

        $('#avatar-max-size').html(maxSizeAllowed);

        $("#file-upload-form button").on("click", function (e) {
            var files = $("#file-upload-form input:file").prop("files");
                  //e.currentTarget.files;

            for (var x in files) {
                if (files[x].name != "item" && typeof files[x].name != "undefined") {
                    if (files[x].size <= maxSizeAllowed * 1024 * 1024) {
                        // Submit the selected file
                        $('#file-upload-form .upload-file-notice').removeClass('bg-danger');
                        $('#file-upload-form').submit();
                    } else {
                        // File too large
                        $('#file-upload-form .upload-file-notice').addClass('bg-danger');
                    }
                }
            }
        });
    }
});

function initFileUpload() {
    $('#file-upload-form').ajaxForm({
        beforeSend: function () {
            updateProgress(0);
            $('#file-upload-form').addClass('hidden');
        },
        uploadProgress: function (event, position, total, percentComplete) {
            updateProgress(percentComplete);
        },
        success: function (data) {
            updateProgress(100);
            if (data.success === false) {
                $('#status').html(data.errorMessage);
            } else {
                //$('#preview-pane .preview-container img').attr('src', data.fileName);

                //var img = $('#crop-avatar-target');
                //img.attr('src', data.fileName);

                //$('#avatar-upload-box').addClass('hidden'); // ToDo - Remove if you want to keep the upload box


                //initAvatarCrop(img);

               // $('#avatar-crop-box').removeClass('hidden');
                //alert(@General.ProfileImageUpdate);
            }
        },
        complete: function (xhr) {
        }
    });
}

function updateProgress(percentComplete) {
    $('.upload-percent-bar').width(percentComplete + '%');
    $('.upload-percent-value').html(percentComplete + '%');
    if (percentComplete === 0) {
        $('#upload-status').empty();
        $('.upload-progress').removeClass('hidden');
    }
}