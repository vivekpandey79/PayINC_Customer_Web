"use strict";
// Class definition

var KTDropzoneDemo = function () {
    // Private functions
    var demo1 = function () {
        // single file upload
        $('#kt_dropzone_1').dropzone({
            url: "/OnBoarding/ManualForm/UploadPancard",
            uploadMultiple: false,
            timeout: 180000,
            autoProcessQueue: false,
            paramName: "pan_card",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/*",
            init: function () {
                var dzClosure = this; // Makes sure that 'this' is understood inside the functions below.
                // for Dropzone to process the queue (instead of default form behavior):
                document.getElementById("btn_fetch_pan").addEventListener("click", function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    dzClosure.processQueue();
                    $("#btn_fetch_pan").attr('disabled','disabled');
                    $("#btn_fetch_pan").addClass("spinner spinner-white spinner-right");
                });

                //send all the form data along with the files:
                this.on("sendingmultiple", function (data, xhr, formData) {
                    formData.append("firstname", jQuery("#firstname").val());
                    formData.append("lastname", jQuery("#lastname").val());
                });
            },
            success: function (file, response) {
                document.getElementById("btn_fetch_pan").removeAttribute("disabled");
                document.getElementById("btn_fetch_pan").classList.remove("spinner");
                if (response.success) {
                    $("#lblPANPercent").text(response.matchData.panNumber);
                    $("#lblFNamePercent").text(response.matchData.fatherName);
                    $("#lblNamePercent").text(response.matchData.fullName);
                    $("#txtfetchPanNumber").val(response.responseData.panNumber);
                    $("#txtPanFullName").val(response.responseData.name);
                    $("#txtPanFatherName").val(response.responseData.fatherName);
                    $("#txtPanDOB").val(response.responseData.dob);
                    $("#dl_dob").val(response.responseData.dob);
                    document.getElementById("dl_dob").valueAsDate = new Date(response.responseData.dob);
                    $("#pan_fetch_section").show();
                    $("#upload_section").hide();
                }
                else {
                    toastr.error(response.errorMessage, "Alert");
                }
                this.removeAllFiles();
            },
            error: function (file, response) {
                document.getElementById("btn_fetch_pan").removeAttribute("disabled");
                document.getElementById("btn_fetch_pan").classList.remove("spinner");
                toastr.error("Uploading Cancelled", "Alert");
                this.removeAllFiles();
            }
        });

        $('#kt_dropzone_2').dropzone({
            url: "/OnBoarding/ManualForm/UploadVoterID",
            paramName: "file1",
            maxFiles: 1,
            timeout: 180000,
            maxFilesize: 10,
            addRemoveLinks: true,
            autoProcessQueue: false,
            acceptedFiles: "image/*",
        });

        $('#kt_dropzone_3').dropzone({
            url: "/OnBoarding/ManualForm/UploadVoterID",
            paramName: "file2",
            maxFiles: 1,
            maxFilesize: 10,
            addRemoveLinks: true,
            autoProcessQueue: false,
            acceptedFiles: "image/*",
        });

        $('#kt_dropzone_4').dropzone({
            url: "/OnBoarding/ManualForm/UploadDL",
            paramName: "file2",
            uploadMultiple: false,
            maxFiles: 1,
            timeout: 180000,
            maxFilesize: 10,
            addRemoveLinks: true,
            autoProcessQueue: false,
            acceptedFiles: "image/*",
            init: function () {
                var dzClosure = this;
                document.getElementById("btn_fetch_dl").addEventListener("click", function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    dzClosure.processQueue();
                    $("#btn_fetch_dl").attr('disabled', 'disabled');
                    $("#btn_fetch_dl").addClass("spinner spinner-white spinner-right");
                });
                this.on("sending", function (data, xhr, formData) {
                    formData.append("dlnumber", jQuery("#txtdlnumber").val());
                    formData.append("dldob", jQuery("#dl_dob").val());
                });
            },
            success: function (file, response) {
                document.getElementById("btn_fetch_dl").removeAttribute("disabled");
                document.getElementById("btn_fetch_dl").classList.remove("spinner");
                if (response.success) {
                    if (parseFloat(response.responseData.nameMatchCriteria.name1_vs_name2_matchScore) <= parseFloat(0.5)) {
                        toastr.error("DL Not Matching with PAN Details", "Alert");
                        $("#lbl_match_panname").text(response.responseData.nameMatchCriteria.name2);
                        $("#lbl_match_dlname").text(response.responseData.nameMatchCriteria.name1);
                        $("#lbl_match_percentage").text((parseFloat(response.responseData.nameMatchCriteria.name1_vs_name2_matchScore) * 100));
                        $('#pan_dl_comp_modal').modal('show');
                        this.removeAllFiles();
                        return;
                    }
                    $("#lbl_match_panname").text(response.responseData.nameMatchCriteria.name2);
                    $("#lbl_match_dlname").text(response.responseData.nameMatchCriteria.name1);
                    $("#lbl_match_percentage").text((parseFloat(response.responseData.nameMatchCriteria.name1_vs_name2_matchScore) * 100));
                    $("#imgKYC1").attr("src", response.responseData.faceMatchCriteria.imgURL1);
                    $("#imgKYC2").attr("src", response.responseData.faceMatchCriteria.imgURL2);
                    $("#lbl_face_match_percent").text(response.responseData.faceMatchCriteria.matchPercentage);
                    $('#pan_dl_comp_modal').modal('show');
                    $("#dl_fetch_form").show();
                    $("#dl_section").hide();
                    $("#txtFetchDLNo").val(response.responseData.dlNumber);
                    $("#txtFetchDLName").val(response.responseData.name);
                    $("#txtFetchAddress").val(response.responseData.address.address);
                    $("#txtFetchCity").val(response.responseData.address.city);
                    $("#txtFetchState").val(response.responseData.address.state);
                }
                else {
                    toastr.error(response.errorMessage, "Alert");
                }
                this.removeAllFiles();
            },
            error: function (file, response) {
                toastr.error("Uploading Cancelled", "Alert");
                this.removeAllFiles();
            }
        });
        
        $('#kt_dropzone_5').dropzone({
            url: "/OnBoarding/ManualForm/UploadCancelledCheque",
            paramName: "file2",
            maxFiles: 1,
            maxFilesize: 10,
            addRemoveLinks: true,
            autoProcessQueue: false,
            acceptedFiles: "image/*",
            init: function () {
                var dzClosure = this;
                document.getElementById("btn_fetch_dl").addEventListener("click", function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    if (!$("#form_address").valid()) {
                        return;
                    }
                    dzClosure.processQueue();
                    $("#btn_fetch_dl").attr('disabled', 'disabled');
                    $("#btn_fetch_dl").addClass("spinner spinner-white spinner-right");
                });
                this.on("sending", function (data, xhr, formData) {
                    formData.append("dlnumber", jQuery("#txtdlnumber").val());
                    formData.append("dldob", jQuery("#dl_dob").val());
                });
            },
            success: function (file, response) {
                this.removeAllFiles();
            },
            error: function (file, response) {
                toastr.error("Uploading Cancelled", "Alert");
                this.removeAllFiles();
            }
        });

        $('#btn_fetch_voterId').click(function () {            
            var myDropzone = Dropzone.forElement("#kt_dropzone_2");
            var myDropzone1 = Dropzone.forElement("#kt_dropzone_3");
            myDropzone.processQueue();
            myDropzone1.processQueue();
            
        });
    }

    var demo2 = function () {
        var id = '#kt_dropzone_4';
        var previewNode = $(id + " .dropzone-item");
        previewNode.id = "";
        var previewTemplate = previewNode.parent('.dropzone-items').html();
        previewNode.remove();
        var myDropzone4 = new Dropzone(id, {
            url: "https://keenthemes.com/scripts/void.php",
            parallelUploads: 20,
            previewTemplate: previewTemplate,
            maxFilesize: 1, // Max filesize in MB
            autoQueue: false, // Make sure the files aren't queued until manually added
            previewsContainer: id + " .dropzone-items", // Define the container to display the previews
            clickable: id + " .dropzone-select" // Define the element that should be used as click trigger to select files.
        });

        myDropzone4.on("addedfile", function(file) {
            // Hookup the start button
            file.previewElement.querySelector(id + " .dropzone-start").onclick = function() { myDropzone4.enqueueFile(file); };
            $(document).find( id + ' .dropzone-item').css('display', '');
            $( id + " .dropzone-upload, " + id + " .dropzone-remove-all").css('display', 'inline-block');
        });

        // Update the total progress bar
        myDropzone4.on("totaluploadprogress", function(progress) {
            $(this).find( id + " .progress-bar").css('width', progress + "%");
        });

        myDropzone4.on("sending", function(file) {
            // Show the total progress bar when upload starts
            $( id + " .progress-bar").css('opacity', '1');
            // And disable the start button
            file.previewElement.querySelector(id + " .dropzone-start").setAttribute("disabled", "disabled");
        });

        // Hide the total progress bar when nothing's uploading anymore
        myDropzone4.on("complete", function(progress) {
            var thisProgressBar = id + " .dz-complete";
            setTimeout(function(){
                $( thisProgressBar + " .progress-bar, " + thisProgressBar + " .progress, " + thisProgressBar + " .dropzone-start").css('opacity', '0');
            }, 300)

        });

        // Setup the buttons for all transfers
        document.querySelector( id + " .dropzone-upload").onclick = function() {
            myDropzone4.enqueueFiles(myDropzone4.getFilesWithStatus(Dropzone.ADDED));
        };

        // Setup the button for remove all files
        document.querySelector(id + " .dropzone-remove-all").onclick = function() {
            $( id + " .dropzone-upload, " + id + " .dropzone-remove-all").css('display', 'none');
            myDropzone4.removeAllFiles(true);
        };

        // On all files completed upload
        myDropzone4.on("queuecomplete", function(progress){
            $( id + " .dropzone-upload").css('display', 'none');
        });

        // On all files removed
        myDropzone4.on("removedfile", function(file){
            if(myDropzone4.files.length < 1){
                $( id + " .dropzone-upload, " + id + " .dropzone-remove-all").css('display', 'none');
            }
        });
    }

    var demo3 = function () {
         // set the dropzone container id
         var id = '#kt_dropzone_5';

         // set the preview element template
         var previewNode = $(id + " .dropzone-item");
         previewNode.id = "";
         var previewTemplate = previewNode.parent('.dropzone-items').html();
         previewNode.remove();

         var myDropzone5 = new Dropzone(id, { // Make the whole body a dropzone
             url: "https://keenthemes.com/scripts/void.php", // Set the url for your upload script location
             parallelUploads: 20,
             maxFilesize: 1, // Max filesize in MB
             previewTemplate: previewTemplate,
             previewsContainer: id + " .dropzone-items", // Define the container to display the previews
             clickable: id + " .dropzone-select" // Define the element that should be used as click trigger to select files.
         });

         myDropzone5.on("addedfile", function(file) {
             // Hookup the start button
             $(document).find( id + ' .dropzone-item').css('display', '');
         });

         // Update the total progress bar
         myDropzone5.on("totaluploadprogress", function(progress) {
             $( id + " .progress-bar").css('width', progress + "%");
         });

         myDropzone5.on("sending", function(file) {
             // Show the total progress bar when upload starts
             $( id + " .progress-bar").css('opacity', "1");
         });

         // Hide the total progress bar when nothing's uploading anymore
         myDropzone5.on("complete", function(progress) {
             var thisProgressBar = id + " .dz-complete";
             setTimeout(function(){
                 $( thisProgressBar + " .progress-bar, " + thisProgressBar + " .progress").css('opacity', '0');
             }, 300)
         });
    }

    return {
        // public functions
        init: function() {
            demo1();
            //demo2();
            //demo3();
        }
    };
}();

KTUtil.ready(function() {
    KTDropzoneDemo.init();
});
