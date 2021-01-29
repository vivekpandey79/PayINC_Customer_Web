$(document).ready(function () {
    var _formEl = KTUtil.getById('kt_form');
    var _wizard1 = new KTWizard("kt_wizard", {
        startStep: 1, // initial active step number
        clickableSteps: false  // allow step clicking
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
            data: { paymentType: inputVal },
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
        var inputIfsc = $(this).attr('data-ifscode');
        var bankName = $(this).attr('data-bankname');
        _wizard1.goTo(3);
        var loader = '<div class="spinner spinner-lg spinner-primary mr-15"></div>';
        $("#request_map").html(loader);
        $("#mymodal").modal('hide');
        $.ajax({
            url: '/PaymentManagement/PaymentRequest/GetPaymentReqFields',
            type: "POST",
            data: { bankId: inputVal, ifscCode: inputIfsc, bankName: bankName },
            success: function (data) {
                $("#request_map").html(data);
            },
            error: function (er) {
                $("#request_map").html('');
            }
        });
    });
});






