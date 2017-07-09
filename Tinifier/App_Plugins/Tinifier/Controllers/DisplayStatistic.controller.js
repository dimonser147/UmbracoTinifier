angular.module("umbraco").controller("Tinifier.DisplayStatistic.Controller", function ($scope, $http) {

    $scope.currentRequests = 0;
    $scope.monthlyRequestsLimit = 0;
    $scope.currentImage = 0;
    $scope.amounthOfImages = 0;
    $scope.TotalImages = 0;
    $scope.TotalOptimizedImages = 0;

    google.charts.load("current", { packages: ["corechart"] });
    google.charts.setOnLoadCallback(drawChart);

    function drawChart() {

        $http.get("/umbraco/backoffice/api/Statistic/GetStatistic").then(function(response) {

            if (response.data.tsetting != null) {
                $scope.currentRequests = response.data.tsetting.CurrentMonthRequests;
            }

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

        $scope.TotalImages = response.data.statistic.TotalOriginalImages + response.data.statistic.TotalOptimizedImages;
        $scope.TotalOptimizedImages = response.data.statistic.TotalOptimizedImages;

        return data;
    }

    function createOptions() {

        var options = {
            pieHole: 0.4,
            width: 500,
            height: 350,
            legend: { position: "bottom", alignment: "center" }
        };

        return options;
    }

    $http.get("/umbraco/backoffice/api/State/GetCurrentTinifingState").then(function(response) {

        if (response.data == "null") {
            document.getElementById("tinifierStatus").innerHTML = "Panda is sleeping now";
        } else {
            document.getElementById("statusPanda").src = "../../../../Media/Pictures/runPanda.jpg";
            $scope.currentImage = response.data.CurrentImage;
            $scope.amounthOfImages = response.data.AmounthOfImages;
        }
    });
});