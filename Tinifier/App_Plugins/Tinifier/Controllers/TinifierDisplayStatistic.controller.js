angular.module("umbraco").controller("Tinifier.TinifierDisplayStatistic.Controller", function ($scope, $http, $timeout) {

    $scope.currentRequests = 0;
    $scope.monthlyRequestsLimit = 0;
    $scope.currentImage = 0;
    $scope.amounthOfImages = 0;
    $scope.TotalImages = 0;
    $scope.TotalOptimizedImages = 0;
    $scope.UpdateSeconds = 10;
    $scope.TotalSavedBytes = 0;
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

            var columnChart = new google.visualization.ColumnChart(document.getElementById('daysChart'));
            columnChart.draw(DataColumnChart(response), columnChartOptions());
        });
    }

    function DataColumnChart(response)
    {
        var dataArray = [['Date', 'Number of optimized images']];
        for (var n = 0; n < response.data.history.length; n++) {
            dataArray.push([response.data.history[n].OccuredAt, parseInt(response.data.history[n].NumberOfOptimized)])
        }

        if (dataArray.length === 1){
            dataArray.push(['',0]);
        }
        var data = new google.visualization.arrayToDataTable(dataArray);
        return data;
    }

    function createData(response) {
        var data = google.visualization.arrayToDataTable([
            ["Task", "Hours per Day"],
            ["Images Original", response.data.statistic.TotalOriginalImages],
            ["Images Optimized", response.data.statistic.TotalOptimizedImages]
        ]);
        $scope.TotalImages = response.data.statistic.TotalOriginalImages + response.data.statistic.TotalOptimizedImages;
        $scope.TotalOptimizedImages = response.data.statistic.TotalOptimizedImages;
        $scope.TotalSavedBytes = formatBytes(response.data.statistic.TotalSavedBytes);
        return data;
    }

    function createOptions() {
        var options = {
            pieHole: 0.4,
            height: 350,
            width: 550,
            legend: { position: "bottom", alignment: "center" }
        };
        return options;
    }

    function columnChartOptions() {
        var options = {
            width: 550,
            height: 350,
            legend: { position: "bottom", alignment: "center" },
            bar: { groupWidth: "15%" },
            colors: ["red"]
        };
        return options;
    }

    function formatBytes(a, b) {
        if (0 == a) return "0 Bytes";
        var c = 1e3, d = b || 2, e = ["Bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"], f = Math.floor(Math.log(a) / Math.log(c));
        return parseFloat((a / Math.pow(c, f)).toFixed(d)) + " " + e[f];
    }

    $scope.getData = function () {
        $http.get("/umbraco/backoffice/api/TinifierState/GetCurrentTinifingState").then(function (response) {
            if (response.data == "null") {
                document.getElementById("tinifierStatus").innerHTML = "Panda is sleeping now";
                document.getElementById("statusPanda").src = "/App_Plugins/Tinifier/media/sleeping_panda_by_citruspop-d2v8hdd.jpg";
                document.getElementById("updateSeconds").style.display = "none";
            } else {
                document.getElementById("statusPanda").src = "/App_Plugins/Tinifier/media/runPanda.jpg";
                document.getElementById("tinifierStatus").innerHTML = "";
                document.getElementById("tinifierStatus").innerHTML = "Processing: " + response.data.CurrentImage + " / " + response.data.AmounthOfImages;
                $scope.currentImage = response.data.CurrentImage;
                $scope.amounthOfImages = response.data.AmounthOfImages;
                document.getElementById("updateSeconds").style.display = "block";
                document.getElementById("resetButton").style.display = "inline-block";
            }
        });
    };

    var intTimer, drawTimer, timert;
    $scope.intervalFunction = function () {
        intTimer = $timeout(function () {
            $scope.getData();
            $scope.intervalFunction();
        }, 2000);
    };

    $scope.intervalDrawChartFunction = function () {
        drawTimer = $timeout(function () {
            drawChart();
            $scope.intervalDrawChartFunction();
        }, 10000);
    };

    $scope.decrementTimer = function () {
        if ($scope.UpdateSeconds > 0) {
            $scope.UpdateSeconds--;
        }
        else {
            $scope.UpdateSeconds = 10;
        }
    };

    $scope.timer = function () {
        timert = $timeout(function () {
            $scope.decrementTimer();
            $scope.timer();
        }, 900);
    };

    $scope.timer();
    $scope.intervalFunction();
    $scope.intervalDrawChartFunction();

    $scope.$on('$locationChangeStart', function () {
        $timeout.cancel(intTimer);
        $timeout.cancel(drawTimer);
        $timeout.cancel(timert);
    });

    $scope.stopTinifing = function () {
        $http.delete("/umbraco/backoffice/api/TinifierState/DeleteActiveState").success(function (response) {
            notificationsService.success("Success", "Tinifing successfully stoped!");
        }).error(function (response) {
            notificationsService.error("Error", "Tinifing can`t stop now!");
        });
    };
});