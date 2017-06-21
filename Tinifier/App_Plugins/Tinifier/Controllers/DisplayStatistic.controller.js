angular.module('umbraco').controller('Tinifier.DisplayStatistic.Controller', function ($scope, $http) {

    google.charts.load("current", {packages:["corechart"]});
    google.charts.setOnLoadCallback(drawChart);
   
    function drawChart() {

        $http.get('/umbraco/backoffice/api/Tinifier/GetStatistic').then(function (response) {
           var data = CreateData(response);
           var options = CreateOptions();
           var chart = new google.visualization.PieChart(document.getElementById('chart'));
           chart.draw(data, options);
        });           
    }

    function CreateData(response) {

        var data = google.visualization.arrayToDataTable([
              ['Task', 'Hours per Day'],
              ['Images Total', response.data.TotalNumberOfImages],
              ['Images Optimized', response.data.NumberOfOptimizedImages]
        ]);

        return data;
    }

    function CreateOptions() {

        var options = {
            title: 'Images Statistic',
            pieHole: 0.4,
            width: 900,
            height: 700
        };

        return options;
    }
});