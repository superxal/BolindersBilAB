﻿$(function(){

    $('#removeCars').on('click', function () {
        if (confirm('Bekräfta borttagning'))
        {
            $('#carListTable').submit();
        }
    });

    $("#editBrand").val("");
    $("#editBrand").on("change", function (e) {
        location.href = $("#editBrand").val();
    });
});