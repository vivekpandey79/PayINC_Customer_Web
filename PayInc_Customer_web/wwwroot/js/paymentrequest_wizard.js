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
    })

    $(".showmodal").click(function (e) {
        e.preventDefault();
        var inputVal = $(this).attr('data-item');
        $("#mymodal").modal('show');
        var loader = '<div class="spinner spinner-lg spinner-primary mr-15"></div>';
        $("#data_panel").html(loader);
        $.ajax({
            url: '/PaymentManagement/PaymentRequest/PaymentType',
            type: "POST",
            data: { paymentType:inputVal },
            success: function (data) {
                $("#data_panel").html(data);
            },
            error: function (er) {
                $("#mymodal").modal('show');
                $("#data_panel").html('');
            }
        });
    });
    $(".payment-mode").click(function (e) {
        e.preventDefault();
        var inputVal = $(this).attr('data-item');
        _wizard1.goTo(2);
        var loader = '<div class="spinner spinner-lg spinner-primary mr-15"></div>';
        $("#bank_list").html(loader);
        $("#mymodal").modal('hide');
        $.ajax({
            url: '/PaymentManagement/PaymentRequest/GetCompanyBank',
            type: "POST",
            data: { paymentType: inputVal },
            success: function (data) {
                $("#bank_list").html(data);
            },
            error: function (er) {
                //$("#mymodal").modal('show');
                //$("#data_panel").html('');
            }
        });
    });

    $(".showbank").click(function (e) {
        e.preventDefault();
        var inputVal = $(this).attr('data-item');
        _wizard1.goTo(3);
        var loader = '<div class="spinner spinner-lg spinner-primary mr-15"></div>';
        $("#request_map").html(loader);
        $("#mymodal").modal('hide');
        $.ajax({
            url: '/PaymentManagement/PaymentRequest/GetPaymentReqFields',
            type: "POST",
            data: { bankId: inputVal },
            success: function (data) {
                $("#request_map").html(data);
            },
            error: function (er) {
                $("#request_map").html('');
            }
        });
    });
    $("#form_request_fields").submit(function (e) {
        e.preventDefault();
        if (!$(this).valid()) {
            return;
        }
        var url = this.action;    
        $.ajax({
            url: url,
            type: "POST",
            data: $(this).serialize(),
            success: function (data) {
                $("#acknowledge").html(data);
                _wizard1.goTo(4);
            },
            error: function (er) {
                $("#acknowledge").html('');
            }
        });
    });
})