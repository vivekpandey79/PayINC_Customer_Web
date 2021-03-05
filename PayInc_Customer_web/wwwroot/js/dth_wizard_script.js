$(document).ready(function () {
    var _validations = [];
    var _formEl = KTUtil.getById('kt_form');
    _validations.push(FormValidation.formValidation(
        _formEl,
        {
            fields: {
                subscriberno: {
                    validators: {
                        notEmpty: {
                            message: 'Please enter Subscriber Number/Account Number'
                        },
                        stringLength: {
                            min: 4,
                            max: 15,
                            message: 'Please enter valid Subscriber Number/Account Number'
                        },
                        regexp: {
                            regexp: /^[0-9]+$/,
                            message: 'Please enter valid Subscriber Number/Account Number'
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
                operator: {
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
        var validator = _validations[0]; // get validator for currnt step
        validator.validate().then(function (status) {
            if (status === 'Valid') {
                _wizard1.goTo(2);
            }
        });
    })
    $("#form_operator").submit(function (e) {
        e.preventDefault();
        if (!$(this).valid()) {
            return;
        }
        _wizard1.goTo(3);
        GetDTHPlan($("#txtAccountNo").val(), $("#ddlOperator option:selected").text());
    });

    function GetDTHPlan(accountNo, operatorName) {
        if (accountNo === "") {
            return;
        }
        if (operatorName === "") {
            return;
        }
        var loader = '<div class="spinner spinner-lg spinner-primary mr-15"></div>';
        $("#customer_details_panel").html(loader);
        $.ajax({
            url: '/Recharge/DTH/GetDTHPlans',
            type: "POST",
            data: { accountName: accountNo, operatorName: operatorName },
            success: function (data) {
                $("#customer_details_panel").html(data);
            },
            error: function (er) {
                $("#customer_details_panel").html('');
            }
        });
    }
    $('.prev-step').click(function (e) {
        _wizard1.goPrev();
    });
    $("#form_recharge").submit(function (e) {
        e.preventDefault();
        if ($(this).valid()) {
            $("#lblMobileNo").text($("#txtAccountNo").val());
            $("#lblOperator").text($("#ddlOperator option:selected").text());
            $("#hdnOperator").val($("#ddlOperator option:selected").text());
            $("#lblAmount").text($("#txtamount").val());
            $("#hdnMobileNumber").val($("#txtAccountNo").val());
            $("#hdnOperatorId").val($("#ddlOperator").val());
            $("#hdnAmount").val($("#txtamount").val());
            $("#digit-1").focus();
            $('#mymodal').modal('show');
        }
    })
   
    //$("#btn_step3").click(function (e) {
    //    e.preventDefault();
    //    $('#mymodal').modal('hide'); 
    //    var validator = _validations[2]; // get validator for currnt step
    //    validator.validate().then(function (status) {
    //        if (status === 'Valid') {
    //            $.ajax({
    //                url: '/Recharge/DTH/DoDTHTransaction',
    //                type: "POST",
    //                data: {},
    //                success: function (data) {
    //                    alert(data)
    //                    $("#customer_details_panel").html(data);
    //                },
    //                error: function (er) {
    //                    $("#customer_details_panel").html('');
    //                }
    //            });
    //            _wizard1.goTo(3);

    //        }
    //    });
    //});
})