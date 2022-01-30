//$(document).ready(function () {
//    Initialize();
//}); 

//function Initialize()
//{
//    google.maps.visualRefresh = true;
//    var CasaGaillard = new google.maps.LatLng(39.608607, 2.6697645); 

//    var mapOptions = {
//        zoom: 18,
//        center: CasaGaillard,
//        mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
//    };  

//    var map = new google.maps.Map(document.getElementById("map"), mapOptions);

//    var marker = new google.maps.Marker({
//        position: CasaGaillard,
//        map: map,
//        title: 'Casa Gaillard S.A.'
//    });    
//    marker.setIcon('/Content/images/logo-casa-white.png') 
//}

function modalvehiculos(url, title) {
    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $('#editmodal .modal-body').html(res);
            $('#editmodal .modal-title').html(title);
            $('#editmodal').modal('show');
        }
    })
};