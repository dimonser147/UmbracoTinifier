angular.module("umbraco").controller("Tinifier.DisplayStatistic.Controller", function($scope, $http) {

        $scope.currentRequests = 0;
        $scope.monthlyRequestsLimit = 0;

        google.charts.load("current", { packages: ["corechart"] });
        google.charts.setOnLoadCallback(drawChart);

        function drawChart() {

            $http.get("/umbraco/backoffice/api/Tinifier/GetStatistic").then(function(response) {
                $scope.currentRequests = response.data.tsetting.CurrentMonthRequests;
                $scope.monthlyRequestsLimit = response.data.monthlyRequestsLimit;
                var data = createData(response);
                var options = createOptions();
                var chart = new google.visualization.PieChart(document.getElementById("chart"));
                chart.draw(data, options);
            });
        }

        function createData(response) {

            var data = google.visualization.arrayToDataTable([
                ["Task", "Hours per Day"],
                ["Images Original", response.data.statistic.TotalOriginalImages],
                ["Images Optimized", response.data.statistic.TotalOptimizedImages]
            ]);

            return data;
        }

        function createOptions() {

            var options = {
                title: "Optimization Statistics",
                pieHole: 0.4,
                width: 750,
                height: 550,
                backgroundColor: "#f5f5f5"
            };

            return options;
        }
});