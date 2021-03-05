$(document).ready(function (e) {
	var _wizard1 = "";
	if ($("#step_num").val() === "") {
		_wizard1 = new KTWizard("kt_wizard", {
			startStep: 1,
			clickableSteps: false
		});
	}
	else {
		_wizard1 = new KTWizard("kt_wizard", {
			startStep: 6,
			clickableSteps: false
		});
		if ($("#step_num").val() === "6") {
			$.ajax({
				url: "/OnBoarding/ManualForm/BankDetails",
				type: "POST",
				data: {},
				success: function (data) {
					$("#panel_all_details").html(data);
					_wizard1.goTo(6);
				}
			});
		}
		if ($("#step_num").val() === "5") {
			GetBankList();
			$.ajax({
				url: "/OnBoarding/ManualForm/BasicDetails",
				type: "POST",
				data: {},
				success: function (data) {
					if (data.bankData !== "") {
						$("#ddlBank").val(data.bankData.bankId);
						$("#accountname").val(data.bankData.accountname);
						$("#bankaccount").val(data.bankData.bankaccount);
						$("#txtifsccode").val(data.bankData.ifsccode);
					}
				}
			});
            _wizard1.goTo(5);
		}
		if ($("#step_num").val() === "3") {
			_wizard1.goTo(3);
		}
		if ($("#step_num").val() === "2") {
			_wizard1.goTo(2);
		}
	}

	var _validations = [];
	var initValidation = function () {
		_validations.push(FormValidation.formValidation(
			_formEl,
			{
				fields: {
					firstname: {
						validators: {
							notEmpty: {
								message: 'First name is required'
							}
						}
					},
					lastname: {
						validators: {
							notEmpty: {
								message: 'Last name is required'
							}
						}
					},
					phone: {
						validators: {
							notEmpty: {
								message: 'Phone is required'
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
					address1: {
						validators: {
							notEmpty: {
								message: 'Address is required'
							}
						}
					},
					address2: {
						validators: {
							notEmpty: {
								message: 'Address is required'
							}
						}
					},
					postcode: {
						validators: {
							notEmpty: {
								message: 'Postcode is required'
							}
						}
					},
					city: {
						validators: {
							notEmpty: {
								message: 'City is required'
							}
						}
					},
					state: {
						validators: {
							notEmpty: {
								message: 'State is required'
							}
						}
					},
					country: {
						validators: {
							notEmpty: {
								message: 'Country is required'
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
	};

	$("#form_verify_mobile").submit(function (e) {
		e.preventDefault();
		var btn = KTUtil.getById("btn_verify_mobile");
		KTUtil.addEvent(btn, "click", function () {
			KTUtil.btnWait(btn, "spinner spinner-right spinner-white pr-15", "Please wait");
		});
		var url = this.action;
		if ($(this).valid()) {
			$.ajax({
				url: url,
				type: "POST",
				data: $("#form_verify_mobile").serialize(),
				success: function (data) {
					KTUtil.btnRelease(btn);
					//if (data.success) {
					//	$(".loader").hide();
					//	$("#btn_verify_mobile").removeAttr("disabled");
					//	_wizard.goNext();
					//}
					//else {
					//	$(".loader").hide();
					//	$("#txtPanNumber").val('');
					//	$("#btn_submit_pan").removeAttr("disabled");
					//	toastr.error(data.errorMessage, "Alert");
					//}
					_wizard1.goNext();
				}
			});
        }
		
	});

	$("#form_upload_pan_verify").submit(function (e) {
		e.preventDefault();
		$("#btn_upload_pan").addClass("spinner spinner-right spinner-white pr-15");
		$("#btn_upload_pan").text("Please wait");
		$("#btn_upload_pan").attr("disabled", "disabled");
		var url = this.action;
		$.ajax({
			url: url,
			type: "POST",
			data: $("#form_upload_pan_verify").serialize(),
			success: function (data) {
				$("#btn_upload_pan").removeClass("spinner");
				$("#btn_upload_pan").text("Next");
				$("#btn_upload_pan").removeAttr("disabled");
				_wizard1.goNext();
			}
		});
	});

	$("#form_address").submit(function (e) {
		e.preventDefault();
		var btn = KTUtil.getById("btn_upld_address");
		KTUtil.addEvent(btn, "click", function () {
			KTUtil.btnWait(btn, "spinner spinner-right spinner-white pr-15", "Please wait");
		});
		if (!$(this).valid()) {
			return;
		}
		var url = this.action;
		$.ajax({
			url: url,
			type: "POST",
			data: $("#form_address").serialize(),
			success: function (data) {
				if (data.success) {
					$("#txtBasicName").val(data.responseData.fullName);
					$("#txtBasicFatherName").val(data.responseData.fatherName);
					$("#txtBasicDateOfBirth").val(data.responseData.dateOfBirth);
					$("#txtBasicMobileNumber").val(data.responseData.mobileNumber);

					$("#txtBasicAddress").val(data.responseData.address);
					$("#txtBasicCity").val(data.responseData.city);
					$("#txtBasicState").val(data.responseData.state);
					$("#txtBasicPinCode").val(data.responseData.pinCode);
					$("#ddlBasicArea").append($("<option></option>").val('').html('-- Loading Area --'));
					$.ajax({
						url: '/OnBoarding/ManualForm/GetAreaPinCode',
						type: "POST",
						data: { pincode: $("#txtBasicPinCode").val() },
						success: function (res) {
							if (res.success) {
								$("#ddlBasicArea").empty();
								$("#ddlBasicArea").append($("<option></option>").val('').html('-- Select Area --'));
								$.each(res.responseData, function (data, value) {
									$("#ddlBasicArea").append($("<option></option>").val(value.areaId).html(value.area));
								});
							}
							else {
								$("#ddlBasicArea").empty();
								$("#ddlBasicArea").append($("<option></option>").val('').html('-- No Area Found--'));
								console.log("error at pincode");
							}
						},
						error: function (er) {
							$("#ddlBasicArea").empty();
							$("#ddlBasicArea").append($("<option></option>").val('').html('-- No Area Found --'));
						}
					});

                }
				KTUtil.btnRelease(btn);
				_wizard1.goNext();
			}
		});
	});
	$("#form_basic_details").submit(function (e) {
		e.preventDefault();
		if (!$(this).valid()) {
			return;
		}
		var btn = KTUtil.getById("btn_basic_details");
		KTUtil.addEvent(btn, "click", function () {
			KTUtil.btnWait(btn, "spinner spinner-right spinner-white pr-15", "Please wait");
		});
		var url = this.action;
		$.ajax({
			url: url,
			type: "POST",
			data: { addressDetails: $("#form_address_panel").serialize(), personalDetails: $("#form_personal_details").serialize(), basicDetails: $("#form_basic_details").serialize()  },
			success: function (data) {
				KTUtil.btnRelease(btn);
				GetBankList();
				_wizard1.goNext();
			}
		});
	});
	$("#form_bankdetails").submit(function (e) {
		e.preventDefault();
		if (!$(this).valid()) {
			return;
		}
		var btn = KTUtil.getById("btn_bankdetails");
		KTUtil.addEvent(btn, "click", function () {
			KTUtil.btnWait(btn, "spinner spinner-right spinner-white pr-15", "Please wait");
		});
		var url = this.action;
		$.ajax({
			url: url,
			type: "POST",
			data: $("#form_bankdetails").serialize(),
			success: function (data) {
				KTUtil.btnRelease(btn);
				$("#panel_all_details").html(data);
				_wizard1.goNext();
			}
		});
	});
	function GetBankList() {
		$.ajax({
			url: "/OnBoarding/ManualForm/GetBankList",
			type: "POST",
			success: function (res) {
				$("#ddlBank").empty();
				$("#ddlBank").append($("<option></option>").val('').html('-- Select Bank --'));
				$.each(res, function (data, value) {
					$("#ddlBank").append($("<option></option>").val(value.bankId + "~" + value.bankName).html(value.bankName));
				})
			}
		});
	}

	$("#btn_fetch_dl").submit(function (e) {
		e.preventDefault();
		
	})
	$('#chk_same_address').change(function () {
		if (this.checked) {
			$("#txtOutletAddress").val($("#txtBasicAddress").val());
			$("#txtOutletState").val($("#txtBasicState").val());
			$("#txtOutletCity").val($("#txtBasicCity").val());
			$("#txtOutletDistrict").val($("#txtBasicDist").val());
			$("#txtOutletLandmark").val($("#txtBasicLandmark").val());
			$("#txtOutletPinCode").val($("#txtBasicPinCode").val());
			$("#ddlOutletArea").append($("<option></option>").val('').html('-- Loading Area --'));
			$.ajax({
				url: '/OnBoarding/ManualForm/GetAreaPinCode',
				type: "POST",
				data: { pincode: $("#txtOutletPinCode").val() },
				success: function (res) {
					if (res.success) {
						$("#ddlOutletArea").empty();
						$("#ddlOutletArea").append($("<option></option>").val('').html('-- Select Area --'));
						$.each(res.responseData, function (data, value) {
							$("#ddlOutletArea").append($("<option></option>").val(value.areaId).html(value.area));
						});
					}
					else {
						$("#ddlOutletArea").empty();
						$("#ddlOutletArea").append($("<option></option>").val('').html('-- No Area Found--'));
						console.log("error at pincode");
					}
				},
				error: function (er) {
					$("#ddlOutletArea").empty();
					$("#ddlOutletArea").append($("<option></option>").val('').html('-- No Area Found --'));
				}
			});
		}
		else {
			$("#txtOutletAddress").val('');
			$("#txtOutletState").val('');
			$("#txtOutletCity").val('');
			$("#txtOutletDistrict").val('');
			$("#txtOutletLandmark").val('');
			$("#txtOutletPinCode").val('');
		}
	});
	$('input[type=radio][name=rdbaddress]').change(function () {
		if (this.value === '1') {
			$("#voter_section").show();
			$("#dl_section").hide();
		}
		else if (this.value === '2') {
			$("#voter_section").hide();
			$("#dl_section").show();
		}
	});

	$("#btn_personal_det").click(function (e) {
		e.preventDefault();
		if ($("#form_personal_details").valid()) {
			$("#collapseTwo4").collapse("show");
		}
	});
	$("#btn_address_det").click(function (e) {
		e.preventDefault();
		if ($("#form_address_panel").valid()) {
			$("#collapseThree4").collapse("show");
		}
	});

	$("#txtBasicPinCode").on("keyup change", function (e) {
		e.preventDefault();
		$("#pincode_div").removeClass("spinner spinner-success spinner-right");
		if ($("#txtBasicPinCode").val().length === 6) {
			$("#pincode_div").addClass("spinner spinner-success spinner-right");
			$("#lbl_pincode_validation").text("");
			$.ajax({
				url: '/OnBoarding/ManualForm/GetAreaPinCode',
				type: "POST",
				data: { pincode: $("#txtBasicPinCode").val() },
				success: function (res) {
					$("#pincode_div").removeClass("spinner spinner-success spinner-right");
					if (res.success) {
						$("#ddlBasicArea").empty();
						$("#ddlBasicArea").append($("<option></option>").val('').html('-- Select Area --'));
						$.each(res.responseData, function (data, value) {
							$("#ddlBasicArea").append($("<option></option>").val(value.areaId).html(value.area));
						});
					}
					else {
						$("#lbl_pincode_validation").text("Invalid Pincode");
					}
				}
			});
        }
	});
	$("#txtOutletPinCode").on("keyup change", function (e) {
		e.preventDefault();
		$("#pincode_div1").removeClass("spinner spinner-success spinner-right");
		if ($("#txtOutletPinCode").val().length === 6) {
			$("#pincode_div1").addClass("spinner spinner-success spinner-right");
			$("#lbl_pincode_validation1").text("");
			$.ajax({
				url: '/OnBoarding/ManualForm/GetAreaPinCode',
				type: "POST",
				data: { pincode: $("#txtOutletPinCode").val() },
				success: function (res) {
					$("#pincode_div1").removeClass("spinner spinner-success spinner-right");
					if (res.success) {
						$("#ddlOutletArea").empty();
						$("#ddlOutletArea").append($("<option></option>").val('').html('-- Select Area --'));
						$.each(res.responseData, function (data, value) {
							$("#ddlOutletArea").append($("<option></option>").val(value.areaId).html(value.area));
						});
					}
					else {
						$("#lbl_pincode_validation1").text("Invalid Pincode");
					}
				}
			});
		}
	});

	$("#form_video_verification").submit(function (e) {
		e.preventDefault();
		var btn = KTUtil.getById("btn_verify_video");
		KTUtil.addEvent(btn, "click", function () {
			KTUtil.btnWait(btn, "spinner spinner-right spinner-white pr-15", "Please wait");
		});
		$("#btn_verify_video").attr("disabled", "disabled");
		var url = this.action;
		$.ajax({
			url: url,
			type: "POST",
			data: $("#form_video_verification").serialize(),
			success: function (data) {
				if (data.success) {
					
					window.location = data.responseData.result.videoUrl;
                }
				KTUtil.btnRelease(btn);
			}
		});
	});
	
});


