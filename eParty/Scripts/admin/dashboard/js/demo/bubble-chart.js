document.addEventListener("DOMContentLoaded", function () {
    var canvas = document.getElementById('bubbleChart');
    if (!canvas) return;
    var ctx = canvas.getContext('2d');

    // Dữ liệu mẫu
    var data = [
        { x: 1, y: 1, r: 20, label: "Quận 1" },
        { x: 2, y: 1, r: 15, label: "Quận 3" },
        { x: 3, y: 2, r: 25, label: "Thủ Đức" },
        { x: 4, y: 2, r: 10, label: "Quận 7" },
        { x: 5, y: 3, r: 30, label: "Bình Thạnh" }
    ];

    // Mảng màu tương ứng
    var colors = [
        'rgba(255, 99, 132, 0.6)',
        'rgba(54, 162, 235, 0.6)',
        'rgba(255, 206, 86, 0.6)',
        'rgba(75, 192, 192, 0.6)',
        'rgba(153, 102, 255, 0.6)'
    ];

    var borderColors = [
        'rgba(255, 99, 132, 1)',
        'rgba(54, 162, 235, 1)',
        'rgba(255, 206, 86, 1)',
        'rgba(75, 192, 192, 1)',
        'rgba(153, 102, 255, 1)'
    ];

    var myBubbleChart = new Chart(ctx, {
        type: 'bubble',
        data: {
            datasets: [{
                label: 'Khu vực TP.HCM',
                data: data,
                backgroundColor: colors,
                borderColor: borderColors,
                borderWidth: 1,
                hoverBackgroundColor: colors.map(c => c.replace('0.6', '0.9')), // hover đậm hơn
                hoverBorderColor: borderColors
            }]
        },
        options: {
            legend: { display: false },
            tooltips: {
                enabled: true,
                callbacks: {
                    label: function (tooltipItem, data) {
                        var dataset = data.datasets[tooltipItem.datasetIndex];
                        var item = dataset.data[tooltipItem.index];
                        return item.label;
                    }
                }
            },
            scales: {
                xAxes: [{ display: false, ticks: { min: 0, max: 6 } }],
                yAxes: [{ display: false, ticks: { min: 0, max: 4 } }]
            },
            animation: { duration: 1000, easing: 'easeOutQuart' },
            responsive: true,
            maintainAspectRatio: false
        }
    });


});
