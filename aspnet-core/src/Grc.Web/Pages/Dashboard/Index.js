(function () {
    $(document).ready(function () {
        console.log("Dashboard initialized!");

        // Compliance Trend Chart
        const complianceTrendCtx = document.getElementById('complianceTrendChart');
        if (complianceTrendCtx) {
            new Chart(complianceTrendCtx, {
                type: 'line',
                data: {
                    labels: ['يناير', 'فبراير', 'مارس', 'أبريل', 'مايو', 'يونيو'],
                    datasets: [{
                        label: 'نسبة الامتثال',
                        data: [65, 70, 75, 80, 83, 85],
                        borderColor: 'rgb(75, 192, 192)',
                        backgroundColor: 'rgba(75, 192, 192, 0.2)',
                        tension: 0.4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: true,
                            position: 'top'
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            max: 100
                        }
                    }
                }
            });
        }

        // Risk Distribution Chart
        const riskDistributionCtx = document.getElementById('riskDistributionChart');
        if (riskDistributionCtx) {
            new Chart(riskDistributionCtx, {
                type: 'pie',
                data: {
                    labels: ['منخفض', 'متوسط', 'عالي', 'حرج'],
                    datasets: [{
                        data: [30, 40, 20, 10],
                        backgroundColor: [
                            'rgba(75, 192, 192, 0.8)',
                            'rgba(255, 206, 86, 0.8)',
                            'rgba(255, 159, 64, 0.8)',
                            'rgba(255, 99, 132, 0.8)'
                        ],
                        borderColor: [
                            'rgba(75, 192, 192, 1)',
                            'rgba(255, 206, 86, 1)',
                            'rgba(255, 159, 64, 1)',
                            'rgba(255, 99, 132, 1)'
                        ],
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: true,
                            position: 'bottom'
                        }
                    }
                }
            });
        }
    });
})();

