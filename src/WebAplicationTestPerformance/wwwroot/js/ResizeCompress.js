$(document).ready(function () {

    UnoDropZone.init();

    window.formData = new FormData(document.forms[0]);
    var time_start = new Date().getTime();

    $("#btnResizeCompress").click(
        function (event) {
            
            var imgs = document.getElementsByName("img");            

            for (var i = 0; i < imgs.length; i++) {

                console.log("Process start...");
                time_start = new Date().getTime();

                console.log("File Name: " + imgs[i].id);
                Resize(imgs[i]);//Resize & Compress
            }

            return;
        }

    );

    $("#btnSendImage").click(
            function (e) {

                var imgs = document.getElementsByName("img");

                console.log("SEND Process start...");

                var time_start = new Date().getTime();                

                //for (var i = 0; i < imgs.length; i++) {

                //    urltoFile(imgs[i].src, imgs[i].id, imgs[i].id).then(function (file) {
                //        formData.append(file.name, file);
                //    });

                //};


                console.log("Send Image");
                SendImage();

                var duration = new Date().getTime() - time_start;
                console.log('END process finished... in: ' + duration + 'ms');

            }
        );

    function Resize(source_image) {

        console.log("Resize Start...");

        if (source_image.src == "") {
            alert("Por favor, selecione uma imagem.");
            return false;
        }

        //Canvas
        var canvas = document.createElement('canvas');
        var ctx = canvas.getContext('2d');

        //Canvas need new image
        var img = new Image();

        img.onload = drawImageScaled.bind(null, img, ctx);
        img.src = source_image.src;

        function drawImageScaled(img, ctx) {

            $("#ResO" + source_image.id).html(" "+ img.width + " x " + img.height);

            var canvas = ctx.canvas;
            canvas.width = 800;
            canvas.height = 600;

            $("#ResT" + source_image.id).html(" "+ canvas.width + " x " + canvas.height);

            var hRatio = canvas.width / img.width;
            var vRatio = canvas.height / img.height;

            var ratio = Math.min(hRatio, vRatio);
            
            console.log("ratio "+ratio);

            var centerShift_x = (canvas.width - img.width * ratio) / 2;
            var centerShift_y = (canvas.height - img.height * ratio) / 2;

            ctx.clearRect(0, 0, canvas.width, canvas.height);
            ctx.drawImage(img, 0, 0, img.width, img.height,
                               centerShift_x, centerShift_y, img.width * ratio, img.height * ratio);

            var t = canvas.toDataURL('image/jpg');
            source_image.src = t;

            console.log("Resize Complete...");
            Compress(source_image, source_image.id);
        }

    }

    function Compress(source_image, id) {

        console.log("Compress Start...");

        var quality = parseInt(70);

        if (source_image.src == "") {
            alert("Por favor, selecione uma imagem.");
            return false;
        }

        //Canvas need new image
        var img = new Image();
        img.src = source_image.src;

        img.onload = function () {
            source_image.src = jic.compress(img, quality, "jpg").src;

            console.log("Compress Complete...");
            ImgtoFile(source_image.src, id);
        }
        
        //ImgtoFile(source_image.src,id);
    }

    function ImgtoFile(src, id) {

        document.getElementById(id).src = src;
        
        srcToFile(src, id, "data:image/jpg;base64").then(function (file) {

            var duration = new Date().getTime() - time_start;
            console.log('process finished... in: ' + duration + 'ms');
                        
            $("#SizeT" + id).html(" "+ UnoDropZone.util.bytesToSize(file.size));

            window.formData.append(file.name, file);
                        
            $('[name="info"]').each(function () {
                console.log($(this));
                $(this).hide().removeClass("hidden").fadeIn();
            });


        });
    }

    function SendImage() {
                
        $.ajax({
            //url: "http://localhost:51629/UploadMultipartUsingReader",
            url: "http://localhost:51629/api/Upload/Large",
            type: 'POST',
            data: window.formData,
            
            processData: false,
            contentType: false,
            
            success: function (data) {
                console.log("boa!");
            },
            error: function () {
                console.log("not so boa!");
            }
        });

    }

    function srcToFile(src, fileName, mimeType) {
        
        return (fetch(src)
            .then(function (res) { return res.arrayBuffer(); })
            .then(function (buf) { return new File([buf], fileName, { type: mimeType }); })
        );
    }

});




