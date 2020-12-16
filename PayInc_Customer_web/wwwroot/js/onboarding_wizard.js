$(document).ready(function () {
    var _validations = [];
    var _formEl = KTUtil.getById('step1_form');

    _validations.push(FormValidation.formValidation(
        _formEl,
        {
            fields: {
                mobilenumber: {
                    validators: {
                        notEmpty: {
                            message: 'Please enter mobile number'
                        },
                        stringLength: {
                            min: 10,
                            max: 15,
                            message: 'Please enter valid mobile number'
                        },
                        regexp: {
                            regexp: /^[0-9]+$/,
                            message: 'Please enter valid mobile number'
                        }
                    }
                }
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap()
            }
        }
    ));

    _validations.push(FormValidation.formValidation(
        _formEl,
        {
            fields: {
                // Step 2
                panNumber: {
                    validators: {
                        choice: {
                            min: 1,
                            message: 'Please select at DTH Opterator.'
                        }
                    }
                }
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap()
            }
        }
    ));

    _validations.push(FormValidation.formValidation(
        _formEl,
        {
            fields: {
                amount: {
                    validators: {
                        notEmpty: {
                            message: 'Please enter amount'
                        }
                    }
                },
                stringLength: {
                    min: 2,
                    max: 15,
                    message: 'Please enter valid amount'
                },
                regexp: {
                    regexp: /^[0-9]+$/,
                    message: 'Please enter valid amount'
                }
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap()
            }
        }
    ));

    var _wizard1 = new KTWizard("kt_wizard", {
        startStep: 1, // initial active step number
        clickableSteps: true  // allow step clicking
    });

    $("#kt_form").submit(function (e) {
        e.preventDefault();
    })
    $("#step1_form").submit(function (e) {
        e.preventDefault();
        var url = this.action;
        var validator = _validations[0];
        validator.validate().then(function (status) {
            if (status === 'Valid') {
                $(".loader").show();
                $("#btn_mobile_verify").attr("disabled", "disabled");
                $.ajax({
                    url: url,
                    type: "POST",
                    data: $("#step1_form").serialize(),
                    success: function (data) {
                        $(".loader").hide();
                        $("#btn_mobile_verify").removeAttr("disabled");
                        try {
                            var checkError = data.errorMessage;
                            if (typeof (checkError) !== "undefined") {
                                toastr.error(checkError, "Alert");
                                return false;
                            }
                            else {
                                $('#step_pan_verfiy').html(data);
                                _wizard1.goNext();
                            }
                        }
                        catch (error) {
                            $('#step_pan_verfiy').html(data);
                            _wizard1.goNext();
                        }
                    }
                });
            }
        });
    })

    $("#submit_pan").submit(function (e) {
        e.preventDefault();
        $(".loader").show();
        $("#btn_submit_pan").attr("disabled", "disabled");
        var url = this.action;
        $.ajax({
            url: url,
            type: "POST",
            data: $("#submit_pan").serialize(),
            success: function (data) {
                if (data.success) {
                    $(".loader").hide();
                    $("#btn_submit_pan").removeAttr("disabled");
                    if (data.success) {
                        $("#fullname").val(data.data.name);
                        $("#fathername").val(data.data.fatherName);
                        $("#dob").val(data.data.dob);
                    }
                    $('#show_pan_details_panel').show();
                    $('#show_pan_verify_panel').hide();
                }
                else {
                    $(".loader").hide();
                    $("#txtPanNumber").val('');
                    $("#btn_submit_pan").removeAttr("disabled");
                    toastr.error(data.errorMessage, "Alert");
                }

                //_wizard1.goNext();
            }
        });
    });

    $("#form_verifypan").submit(function (e) {
        e.preventDefault();
        $(".loader").show();
        $("#btn_verify_pan").attr("disabled", "disabled");
        var url = this.action;
        $.ajax({
            url: url,
            type: "POST",
            data: $("#form_verifypan").serialize(),
            success: function (data) {
                $(".loader").hide();
                $("#btn_verify_pan").removeAttr("disabled");
                try {
                    var checkError = result.errorMessage;
                    if (typeof (checkError) != "undefined") {

                        return false;
                    }
                    else {
                        $('#step_distributor').html(data);
                        _wizard1.goNext();
                    }
                }
                catch (error) {
                    $('#step_distributor').html(data);
                    _wizard1.goNext();
                }
            }
        });
    });

    $("#submit_dist_form").submit(function (e) {
        e.preventDefault();
        $(".loader").show();
        $("#btn_submit_dist").attr("disabled", "disabled");
        var url = this.action;
        $.ajax({
            url: url,
            type: "POST",
            data: $("#submit_dist_form").serialize(),
            success: function (data) {
                $(".loader").hide();
                $("#btn_submit_dist").removeAttr("disabled");
                try {
                    var checkError = result.errorMessage;
                    if (typeof (checkError) != "undefined") {

                        return false;
                    }
                    else {
                        $('#step_verify_method').html(data);
                        _wizard1.goNext();
                    }
                }
                catch (error) {
                    $('#step_verify_method').html(data);
                    _wizard1.goNext();
                }
            }
        });
    });


    $("#btn_step2").click(function (e) {
        e.preventDefault();
        var validator = _validations[1]; // get validator for currnt step
        validator.validate().then(function (status) {
            if (status == 'Valid') {
                _wizard1.goNext();

            } else {

            }
        });
    });
    $('.prev-step').click(function (e) {
        _wizard1.goPrev();
    });
    $("#enter_tpin").click(function (e) {
        e.preventDefault();
        $('#mymodal').modal('show');
    });
    $("#btn_step3").click(function (e) {
        e.preventDefault();
        $('#mymodal').modal('hide');
        var validator = _validations[2]; // get validator for currnt step
        validator.validate().then(function (status) {
            if (status == 'Valid') {

                _wizard1.goNext();

            } else {

            }
        });
    });

    function GotoNextStep() {
        _wizard1.goNext();
    }
})