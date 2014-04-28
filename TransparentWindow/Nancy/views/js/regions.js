var regions = {
    evaluateRegions: function (baseUrl, id) {
        var elements = $(".form-region");
        var regions = [];
        for (var i = 0; i < elements.length; i++) {
            regions.push({
                X: elements[i].clientLeft,
                Y: elements[i].clientTop,
                Width: elements[i].clientWidth,
                Height: elements[i].clientHeight * 2,
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