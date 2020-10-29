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

    
    $("#btn_step1").click(function (e) {
        e.preventDefault();
        // Validate form
        var validator = _validations[0]; // get validator for currnt step
        validator.validate().then(function (status) {
            if (status == 'Valid') {
                _wizard1.goNext();

            } else {
               
            }
        });
        // _wizard1.goNext();
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
})