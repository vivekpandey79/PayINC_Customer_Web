$(document).ready(function (e) {
	CustomerUsage();
	GetNetworkUsage();
	TopTransaction();
	TopLedger();
	GetDayWiseData();
	//GetCharts("");
});
function CustomerUsage() {
	$.ajax({
		url: '/Home/GetCustomerUsage',
		type: "POST",
		data: {},
		success: function (data) {
            if (data!=="") {
				$("#lblCurrentUsage").text(data.currentUsage);
				$("#lblCurrentMonthUsage").text(data.currentMonthUsage);
				$("#lblPaymentRec").text(data.currentPaymentReceived);
				$("#lblPaymentMonthlyRec").text(data.currentMonthPaymentReceived);
            }
		}
	});
}
function GetNetworkUsage() {
	$.ajax({
		url: '/Home/GetNetworkUsage',
		type: "POST",
		data: {},
		success: function (data) {
			if (data !== "") {
				$("#lblNetworkUsage").html('<span class="icon-xl fas fa-xs fa-rupee-sign"></span> '+data.currentNetworkUsage);
				$("#lblNetworkMonthUsage").html('<span class="icon-xl fas fa-xs fa-rupee-sign"></span> '+data.currentNetworkMonthUsage);
				$("#lblNetworkPaymentRec").html('<span class="icon-xl fas fa-xs fa-rupee-sign"></span> '+data.currentNetworkPaymentReceived);
				$("#lblNetworkCommissionRec").html('<span class="icon-xl fas fa-xs fa-rupee-sign"></span> '+data.currentNetworkCommisionReceived);
			}
		}
	});
}
function TopTransaction() {
	$.ajax({
		url: '/Home/GetTopTransaction',
		type: "POST",
		data: {},
		success: function (data) {
			$(".tbl_load").html('');
			if (data !== "") {
				var trHTML = '';
				$.each(data, function (key, value) {
					trHTML +=
						'<tr><td class="py-5">' + value.serviceProviderName +
						'</td><td>' + value.consumerNumber +
						'</td><td>' + value.isoTransactionId +
						'</td><td>' + value.txnDate +
					'</td><td>' + '<span class="icon-xl fas fa-xs fa-rupee-sign"></span> '+value.transactionAmount +
						'</td><td class="text_status">' + value.transactionStatusDescription +'</td></tr>';
				});
				$('#records_table').append(trHTML);
				$('.text_status').each(function () {
					var status = $(this);
                    if (status.text().trim() === "PENDING") {
                        $(this).html('<span class="label label-warning label-dot mr-2"></span><span class="font-weight-bold text-warning">PENDING</span>');
                    }
                    else if (status.text() === "SUCCESS") {
						$(this).html('<span class="label label-success label-dot mr-2"></span><span class="font-weight-bold text-success">SUCCESS</span>');
					} 
					else if (status.text() === "FAILED") {
						$(this).html('<span class="label label-danger label-dot mr-2"></span><span class="font-weight-bold text-success">FAILED</span>');
					} 

				});
				
			}
		},
		error: function (er) {
			$("#tbl_load").html('');
		}
	});
}

function TopLedger() {
	const months = [
		"Jan", "Feb",
		"Mar", "Apr", "May",
		"Jun", "Jul", "Aug",
		"Sep", "Oct",
		"Nov", "Dec"
	]; 
	$.ajax({
		url: '/Home/GetTopLedger',
		type: "POST",
		data: {},
		success: function (data) {
			$("#timeline_transaction").html('');
			if (data !== "") {
				var trHTML = '';
				$.each(data, function (key, value) {
					var date = new Date(value.txnDate);
					var time = date.getHours() + " : " + date.getMinutes();
					var DayMonth = date.getDate() + "-" + months[date.getMonth()];
					if (value.category === "Debit") {
						trHTML += '<div class="timeline-item align-items-start" >'
							+ '<div class="timeline-label font-weight-bolder text-dark-75 font-size-lg">' + time + '<br/>' + '<span class="text-muted">' + DayMonth + '</span>' + '</div>'
							+ '<div class="timeline-badge"><i class="fa fa-genderless text-danger icon-xl"></i></div>'
							+ '<div class="font-weight-mormal font-size-lg timeline-content text-dark pl-3">' + value.message + '</div>'
							+ '</div>';

					}
					else if (value.category === "Credit") {
						trHTML += '<div class="timeline-item align-items-start" >'
							+ '<div class="timeline-label font-weight-bolder text-dark-75 font-size-lg">' + time + '<br/>' + '<span class="text-muted">' + DayMonth + '</span>' + '</div>'
							+ '<div class="timeline-badge"><i class="fa fa-genderless text-success icon-xl"></i></div>'
							+ '<div class="font-weight-mormal font-size-lg timeline-content text-dark pl-3">' + value.message + '</div>'
							+ '</div>';
					}
					else {
						trHTML += '<div class="timeline-item align-items-start" >'
							+ '<div class="timeline-label font-weight-bolder text-dark-75 font-size-lg">' + time + '<br/>' + '<span class="text-muted">' + DayMonth + '</span>' + '</div>'
							+ '<div class="timeline-badge"><i class="fa fa-genderless text-primary icon-xl"></i></div>'
							+ '<div class="font-weight-mormal font-size-lg timeline-content text-dark pl-3">' + value.message + '</div>'
							+ '</div>';
					}

				});
				$('#timeline_transaction').append(trHTML);
			}
			else {
				$("#timeline_transaction").html(' <div class="text-center text-muted">No Transaction Found.</div >');
			}
		},
		error: function (er) {
			$("#timeline_transaction").html(' <div class="text-center text-muted">Connection failed.</div >');
		}
	});
}
function GetCharts(data) {
	var arrUsage = [];
	var arrDate = [];
	if (data !== "") {
		$.each(data, function (key, value) {
			arrUsage.push(value.usageAmount);
			arrDate.push(value.date);
		});
		console.log(arrDate);
	}
	const primary = '#6993FF';
	const apexChart = "#chart_1";
	var options = {
		series: [{
			name: "Usage",
			data: arrUsage
		}],
		chart: {
			height: 350,
			type: 'line',
			zoom: {
				enabled: false
			}
		},
		dataLabels: {
			enabled: false
		},
		stroke: {
			curve: 'straight'
		},
		grid: {
			row: {
				colors: ['#f3f3f3', 'transparent'], // takes an array which will be repeated on columns
				opacity: 0.5
			},
		},
		xaxis: {
			categories: arrDate,
		},
		colors: [primary]
	};

	var chart = new ApexCharts(document.querySelector(apexChart), options);
	chart.render();

	

}

function GetDayWiseData() {
	$.ajax({
		url: '/Home/GetDayWiseUsage',
		type: "POST",
		data: {},
		success: function (data) {
			if (data !== "") {
				GetCharts(data);
			}
		}
	});
}