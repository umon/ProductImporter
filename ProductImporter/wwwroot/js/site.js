$('#upload-input').change(function () {

    var files = $(this)[0].files;
    if (files.length > 0) {
        var file = files[0];
        if (file.type != "application/vnd.ms-excel" &&
            file.type != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
            clearUploadInput();
            alert("sadece .xls ve .xlsx dosyaları yükleyebilirsiniz.");
        }
        else {
            $('#file-text').val(file.name);
        }

    }

});

function clearUploadInput() {
    $('#file-text').val('Dosya seçilmedi');
    $('#upload-input').val('');
    $('#file-text').val('Dosya seçilmedi');

}

function uploadFile() {
    $.ajax({
        url: '/home/upload',
        method: 'POST',
        data: new FormData($('form')[0]),
        processData: false,
        contentType: false,
        success: function (result) {
            var importResults = result.importResults
            $('#upload-results').text(importResults.insertedCount + ' Adet Eklendi, ' + importResults.updatedCount + ' Adet Güncellendi, ' + importResults.failedCount + ' Adet kayıt işlenemedi.');

            $('tbody').html('');

            if (result.processFaults != null || result.processFaults.length > 0) {

                $.each(result.processFaults, function (i, v) {

                    var tableRow = $('<tr>');

                    var tableCell1 = $('<td>', { text: v.rowNumber });
                    var tableCell2 = $('<td>', { text: v.error });

                    tableRow.append(tableCell1);
                    tableRow.append(tableCell2);

                    $('tbody').append(tableRow);

                });
            }
        },
        error: function (error) {
            alert(error.responseText);
        }
    })
}