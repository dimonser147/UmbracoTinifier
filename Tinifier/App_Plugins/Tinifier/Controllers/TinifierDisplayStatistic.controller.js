angular.module("umbraco").controller("Tinifier.TinifierDisplayStatistic.Controller", function ($scope, $http, $timeout) {

    $scope.currentRequests = 0;
    $scope.monthlyRequestsLimit = 0;
    $scope.currentImage = 0;
    $scope.amounthOfImages = 0;
    $scope.TotalImages = 0;
    $scope.TotalOptimizedImages = 0;
    google.charts.load("current", { packages: ["corechart"] });
    google.charts.setOnLoadCallback(drawChart);

    function drawChart() {
        $http.get("/umbraco/backoffice/api/TinifierImagesStatistic/GetStatistic").then(function (response) {
            if (response.data.tsetting != null) {
                $scope.currentRequests = response.data.tsetting.CurrentMonthRequests;
            }
            $scope.monthlyRequestsLimit = response.data.MonthlyRequestsLimit;
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

    $scope.getData = function () {
        $http.get("/umbraco/backoffice/api/TinifierState/GetCurrentTinifingState").then(function (response) {
            if (response.data == "null") {
                document.getElementById("tinifierStatus").innerHTML = "Panda is sleeping now";
                document.getElementById("statusPanda").src = "../../../../Media/Pictures/sleeping_panda_by_citruspop-d2v8hdd.jpg";
            } else {
                document.getElementById("statusPanda").src = "../../../../Media/Pictures/runPanda.jpg";
                $scope.currentImage = response.data.CurrentImage;
                $scope.amounthOfImages = response.data.AmounthOfImages;
            }
        });
    };

    $scope.intervalFunction = function () {
        $timeout(function () {
            $scope.getData();
            $scope.intervalFunction();
        }, 2000)
    };

    $scope.intervalDrawChartFunction = function () {
        $timeout(function () {
            drawChart();
            $scope.intervalDrawChartFunction();
        }, 10000)
    };

    $scope.intervalFunction();
    $scope.intervalDrawChartFunction();
});