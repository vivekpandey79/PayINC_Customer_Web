$(document).ready(function () {
   
    var _wizard1 = new KTWizard("kt_wizard", {
        startStep: 1, // initial active step number
        clickableSteps: true  // allow step clicking
    });
    $(".searchable-dropdown").select2();

    $("#form_operator").submit(function (e) {
        e.preventDefault();
        if ($(this).valid()) {
            $("#btn_step1").addClass("spinner spinner-right spinner-white pr-15");
            $("#btn_step1").attr("disabled", "disabled");
            $.ajax({
                url: '/BBPS/BBPS/InputParams',
                type: "POST",
                data: $("#form_operator").serialize(),
                success: function (data) {
                    $("#step_2_panel").html(data);
                    _wizard1.goTo(2);
                },
                error: function (er) {
                    toastr.error("Internet connection failed","alert");
                }
            });
        }
        
    })
    $("#form_input_params").submit(function (e) {
        e.preventDefault();
        if ($(this).valid()) {
            $("#btn_step2").addClass("spinner spinner-right spinner-white pr-15");
            $("#btn_step2").attr("disabled", "disabled");
            $.ajax({
                url: '/BBPS/BBPS/FetchBill',
                type: "POST",
                data: $("#form_input_params").serialize(),
                success: function (data) {
                    $("#step_3_panel").html(data);
                    _wizard1.goTo(3);
                },
                error: function (er) {
                    toastr.error("Internet connection failed", "alert");
                }
            });
        }
    });
    $("#form_bill_fetch").submit(function (e) {
        e.preventDefault();
        if ($(this).valid()) {
            $("#mymodal").modal('show');

        }
    });
    $("#form_bill_fetch").submit(function (e) {
        e.preventDefault();
        if ($(this).valid()) {
            $("#lblMobileNo").text($(".txtAccountNo").val());
            $("#lblOperator").text($("#ddlOperator option:selected").text());
            $("#lblAmount").text($("#txtamount").val());
            $("#hdnMobileNumber").val($(".txtAccountNo").val());
            $("#hdnOperatorId").val($("#ddlOperator").val());
            $("#hdnAmount").val($("#txtamount").val());
            $("#digit-1").focus();
            $("#mymodal").modal('show');

        }
    });
    $('.prev-step').click(function (e) {
        _wizard1.goPrev();
    });
    $("#form_recharge").submit(function (e) {
        e.preventDefault();
        if ($(this).valid()) {
            $("#lblMobileNo").text($("#txtAccountNo").val());
            $("#lblOperator").text($("#ddlOperator option:selected").text());
            $("#lblAmount").text($("#txtamount").val());
            $("#hdnMobileNumber").val($("#txtAccountNo").val());
            $("#hdnOperatorId").val($("#ddlOperator").val());
            $("#hdnAmount").val($("#txtamount").val());
            $("#digit-1").focus();
            $('#mymodal').modal('show');
        }
    });
})