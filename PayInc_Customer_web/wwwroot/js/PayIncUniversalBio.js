    var domain = '127.0.0.1';
    var scheme = "http";
    //initCapture(url, callback)
    var startPort = 11100;
var endPort = 11100;
    var detectedUrl = "";
function captureBio(serviceType)
    {
        for (var i = startPort; i <= endPort; i++)
        {
            var url = scheme + '://' + domain + ':' + i;
            initDiscovery(url,
                function (err, data) {
                    if (err != null) {
                        alert('Something went wrong: ' + err);
                    } else {

                        //var $doc = $.parseXML(data);

                        parser = new DOMParser();
                        xmlDoc = parser.parseFromString(data, "text/xml");


                        var deviceInfo = xmlDoc.getElementsByTagName("RDService")[0].attributes["info"].value;

                        if (deviceInfo.includes("Morpho")) {
                            detectedUrl = detectedUrl + "/capture";
                        }
                        else if (deviceInfo.includes("Mantra")) {
                            detectedUrl = detectedUrl + "/rd/capture";
                        }
                        else {
                            detectedUrl = detectedUrl + "/rd/capture";
                        }
                        
                           
                       // alert(detectedUrl);

                        //var path = xmlDoc.getElementsByTagName("RDService")[0].childNodes[3].attributes["path"].value;

                        //path = path.replace(detectedUrl.replace('http:/', ''), '');
                        //alert(path);

                        
                        initCapture(detectedUrl,
                            function (err, data) {
                                if (err != null) {
                                    alert('Something went wrong: ' + err);
                                } else {
                                    $('.PidData').val(data);
                                    StartLoading("Fetch");
                                    var url = "";
                                    var formData = "";
                                    if (serviceType === "BE") {
                                        StartLoading("Loading Balance Enquiry");
                                        url = $("#form_balance_enq").attr("action");
                                        formData = $("#form_balance_enq").serialize()
                                    }
                                    else if (serviceType === "W") {
                                        StartLoading("Loading Cash Withdraw");
                                        url = $("#form_withdraw").attr("action");
                                        formData = $("#form_withdraw").serialize()
                                    }
                                    else if (serviceType === "M") {
                                        StartLoading("Loading MINI STATEMENT");
                                        url = $("#form_ministatement").attr("action");
                                        formData = $("#form_ministatement").serialize()
                                    }
                                    else if (serviceType === "AP") {
                                        StartLoading("Loading Aadhar Pay");
                                        url = $("#form_aadharpay").attr("action");
                                        formData = $("#form_aadharpay").serialize()
                                    }
                                    $.ajax({
                                        url: url,
                                        type: "POST",
                                        data: formData,
                                        success: function (data) {
                                            $("#btn_bal_enq").removeClass("spinner spinner-white spinner-right");
                                            $("#btn_bal_enq").removeAttr("disabled");
                                            StopLoading();
                                            try {
                                                var checkError = data.errorMessage;
                                                if (typeof (checkError) !== "undefined") {
                                                    toastr.error(checkError, "Alert");
                                                    return false;
                                                }
                                                else {
                                                    $("#ack_section").html(data);
                                                    $("#mymodal").modal("show");
                                                }
                                            }
                                            catch (error) {
                                                $("#ack_section").html(data);
                                                $("#mymodal").modal("show");
                                            }
                                        },
                                        error: function (er) {
                                            StopLoading();
                                        }
                                    });
                                }
                            }
                        );
                    }
                }
            );
        }
        
       
    }
    var initDiscovery = function (url, callback) {
        var xhr = new XMLHttpRequest();
        xhr.open('RDSERVICE', url, true);
        xhr.responseType = 'text';
        xhr.onload = function () {
            var status = xhr.status;
            if (status == 200) {
                detectedUrl = url;
                callback(null, xhr.response);
            } else {
                callback(status);
            }
        };
        xhr.send();
    };

    var initCapture = function (url, callback) {
        var xhr = new XMLHttpRequest();
        xhr.open('CAPTURE', url, true);
        xhr.responseType = 'text';
        var InputXml = '<?xml version="1.0"?><PidOptions><Opts fCount="1" fType="0" iCount="0" pCount="0" format="0" pidVer="2.0" timeout="10000" wadh="" env="P" /></PidOptions>';
        xhr.onload = function () {
            var status = xhr.status;
            if (status == 200) {
                callback(null, xhr.response);
            } else {
                callback(status);
            }
        };
        xhr.send(InputXml);
    };