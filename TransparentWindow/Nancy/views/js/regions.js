var regions = {
    evaluateRegions: function (baseUrl, id) {
        var elements = $(".form-region");
        var regions = [];
        for (var i = 0; i < elements.length; i++) {
            var $e = $(elements[i]);
            regions.push({
                X: $e.position().left,
                Y: $e.position().top,
                Width: $e.width(),
                Height: $e.height(),
            });
        }

        //DELETE old regions then PUT new regions
        $.ajax({
            url: baseUrl + "screens/regions/" + id,
            type: "PUT",
            contentType: "application/json",
            data: JSON.stringify({ Regions: regions })
        })
        .done(function(data) {
        });
    }
};