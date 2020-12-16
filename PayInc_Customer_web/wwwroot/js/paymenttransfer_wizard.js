var _wizard1 = new KTWizard("kt_wizard", {
    startStep: 1,
    clickableSteps: false
});
$(document).ready(function () {
    
});
$("#form_payment").submit(function (e) {
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
            $("#view_profile").html(data);
            _wizard1.goTo(2);
        },
        error: function (er) {

        }
    });
});
$("#form_showprofile").submit(function (e) {
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
            _wizard1.goTo(3);
        },
        error: function (er) {

        }
    });
});

$("form").submit(function (e) {
    if ($(this).valid()) {
        $(".btn-primary").addClass("spinner spinner-white spinner-right");
        $(".btn-primary").attr("disabled", "disabled");
    }
});