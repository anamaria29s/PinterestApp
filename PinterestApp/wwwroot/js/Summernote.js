$(document).ready(function () {
    var imageAdded = false;

    $('.summernote').summernote({
        height: 300,
        minHeight: 200,
        focus: true,
        toolbar: [
            ['insert', ['picture', 'video']]
        ],
        callbacks: {
            onInit: function () {
                // Verificăm conținutul la început
                checkImageExistence();
            },
            onImageUpload: function (files) {
                // Marchează variabila pentru a ține evidența adăugării imaginilor
                imageAdded = true;
            },
            onChange: function () {
                // Verificăm conținutul la fiecare schimbare
                checkImageExistence();
            },
            onPaste: function () {
                // Verificăm conținutul la fiecare operație de paste
                setTimeout(function () {
                    checkImageExistence();
                }, 100);
            }
        }
    });

    // Interceptăm click-ul pe butonul de submit cu clasa "submitButtonForImage"
    $('.submitButtonForImage').click(function () {
        // Verificăm existența imaginilor înainte de submit
        var content = $('.summernote').summernote('code');
        var containsImages = $(content).find('img').length > 0;

        // Dacă nu există imagini, afișează un mesaj de eroare și oprește submit-ul
        if (!containsImages) {
            alert('Adăugați cel puțin o imagine!');
            return false;
        }

        // Altfel, permite submit-ul formularului
        $(this).closest('form').submit();
    });

    // Funcție pentru a verifica existența imaginilor în conținut
    function checkImageExistence() {
        var content = $('.summernote').summernote('code');
        var containsImages = $(content).find('img').length > 0;

        // Dacă există imagini, actualizează variabila pentru a permite salvarea
        if (containsImages) {
            imageAdded = true;
        }
    }
});
