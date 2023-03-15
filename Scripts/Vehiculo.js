
$(document).ready(function () {
    BindVehiculosData();
});

function BindVehiculosData() {
    $.ajax({
        url: "/Mantenimiento/Vehiculos/GetVehiculos",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result) {
                var html = "";
                $.each(result, function (key, item) {
                    html += '<tr>';
                    html += '<td>' + item.TipoVehiculo + '</td>';
                    html += '<td>' + item.Marca + '</td>';
                    html += '<td>' + item.Modelo + '</td>';
                    html += '<td>' + item.Matricula + '</td>';
                    html += '<td>' + item.Pma + '</td>';
                    html += '<td>' + item.Tara + '</td>';
                    html += '<td>' + item.CargaUtil + '</td>';
                    html += '<td class="text-center"> <a onclick = "return OpenAddOrEditPopup(' + item.ID + ')" class="btn btn-sm btn-outline-primary text-primary" style="z-index: 100;" ><i class="fa fa-sm fa-pen"></i></a>' +
                        '<a  onclick="DeleteVehiculo(' + item.ID + ')" class="btn btn-sm btn-outline-danger text-danger" style="z-index: 100;"><i class="fa fa-sm fa-trash"></i></a></td>';
                    html += '</tr>';
                });
                $('.tbody').html(html);
            }
        },
        error: function (errormessage) {
            alert('error');
        }
    });
}

function AddVehiculo() {
    var res = ValidateUserInput();
    if (res == false) {
        return false;
    }


    var vehiculoObj = {
        ID: $('#hfVehiculoID').val(),
        TipoVehiculoID: $('#selTipoVehiculo').val(),
        Marca: $('#txtMarca').val(),
        Modelo: $('#txtModelo').val(),
        FechaCompra: $('#txtFechaCompra').val(),
        MatriculaVehiculo: $('#txtMatricula').val(),
        ModeloTacografo: $('#txtTacografo').val(),
        Pma: $('#txtPma').val(),
        Tara: $('#txtTara').val(),
        NumBastidor: $('#txtNumBastidor').val(),
        KilometrajeCompra: $('#txtKilometrosCompra').val(),
        NumEjes: $('#txtNumEjes').val(),
        PotenciaCV: $('#txtPotencia').val(),
        TallerHabitualID: $('#selTaller option:selected').val(),
        TipoCombustible: $('#selCombustible').val(),
        Longitud: $('#txtLongitud').val(),
        Anchura: $('#txtAnchura').val(),
        Altura: $('#txtAltura').val(),
        SeguroID: $('#selSeguro option:selected').val(),
    };
    $.ajax({
        url: "/Mantenimiento/Vehiculos/AddVehiculo",
        data: JSON.stringify(vehiculoObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            //populate table with new record
            BindVehiculosData();
            //claer all input and hide model popup
            ClearAllInput();
            $('#VehiculoAddOrEditModal').modal('hide');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}


function OpenAddOrEditPopup(VehiculoId) {
    ClearAllInput();
    $.ajax({
        url: "/Mantenimiento/Vehiculos/GetVehiculoByID?VehiculoID=" + (VehiculoId || null),
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            //debugger;

            //Poblamos los 'select'

            $('#selTipoVehiculo option').remove();                          // borra los <option> añadidos anteriormente.
            $('#selTipoVehiculo').append('<option value="0" selected>Seleccione un tipo de vehículo</option>');
            $.each(result.selTipoVehiculo, function (key, value) {          // recorre los datos enviados para el <select>
                let selOption = '<option value=' + value.Value + ' ';
                if (result.vehiculo && value.Value == result.vehiculo.TipoVehiculo.ID) {       // Marca esta <option> como 'selected' según los datos del vehículo.
                    selOption += 'selected= "true"';
                };
                selOption += '>' + value.Text + '</option>';
                $('#selTipoVehiculo').append(selOption);                    // Añade  el <option> al <select>
            });
            $('#selCombustible option').remove();                          // borra los <option> añadidos anteriormente.
            $('#selCombustible').append('<option value="0" selected>Combustible</option>');
            $.each(result.selCombustible, function (key, value) {          // recorre los datos enviados para el <select>
                let selOption = '<option value=' + value.Value + ' ';
                if (result.vehiculo && value.Value == result.vehiculo.TipoCombustible) {       // Marca esta <option> como 'selected' según los datos del vehículo.
                    selOption += 'selected= "true"';
                };
                selOption += '>' + value.Text + '</option>';
                $('#selCombustible').append(selOption);                    // Añade  el <option> al <select>
            });
            $('#selSeguro option').remove();                          // borra los <option> añadidos anteriormente.
            $('#selSeguro').append('<option value="0" selected>Seguro</option>');
            $.each(result.selSeguro, function (key, value) {          // recorre los datos enviados para el <select>
                let selOption = '<option value=' + value.Value + ' ';
                if (result.vehiculo && value.Value == result.vehiculo.SeguroID) {       // Marca esta <option> como 'selected' según los datos del vehículo.
                    selOption += 'selected= "true"';
                };
                selOption += '>' + value.Text + '</option>';
                $('#selSeguro').append(selOption);                    // Añade  el <option> al <select>
            });

            if (result.vehiculo) {

                // Si 'VehiculoId' no es 'null' poblamos los campos con los valores del vehículo.

                $("#VehiculoAddOrEditModalLabel").text("Actualizar Vehículo");
                $('#hfVehiculoID').val(result.vehiculo.ID);
                $('#txtMarca').val(result.vehiculo.Marca);
                $('#txtModelo').val(result.vehiculo.Modelo || '-');
                if (result.vehiculo.FechaCompra) {
                    let fecha = new Date(parseInt(result.vehiculo.FechaCompra.match(/\d+/)[0]));   // Formatea la fecha para mostrarla correctamente.
                    $('#txtFechaCompra').val(fecha.getDay() + "-" + fecha.getMonth() + "-" + fecha.getFullYear());
                }
                else {
                    $('#txtFechaCompra').val('-');
                }
                $('#txtMatricula').val(result.vehiculo.MatriculaVehiculo || '-');
                $('#txtTacografo').val(result.vehiculo.ModeloTacografo || '-');
                $('#txtPma').val(result.vehiculo.Pma || '-');
                $('#txtTara').val(result.vehiculo.Tara || '-');
                $('#txtNumBastidor').val(result.vehiculo.NumBastidor || '-');
                $('#txtKilometrosCompra').val(result.vehiculo.KilometrajeCompra || '-');
                $('#txtNumEjes').val(result.vehiculo.NumEjes || '-');
                $('#txtPotencia').val(result.vehiculo.PotenciaCV || '-');
                $('#selTaller option:selected').val(result.vehiculo.TallerHabitualID || '-');
                $('#txtLongitud').val(result.vehiculo.Longitud || '-');
                $('#txtAnchura').val(result.vehiculo.Anchura || '-');
                $('#txtAltura').val(result.vehiculo.Altura || '-');

                $('#VehiculoAddOrEditModal').modal('show');                          // Muestra la ventana modal
                $('#btnUpdateVehiculo').show();                                 // muestra el botón ACTUALIZAR
                $('#btnAddVehiculo').hide();                                    // Esconde el botón CREAR

            }
            else {
                $("#VehiculoAddOrEditModalLabel").text("Crear Vehículo");


                $('#VehiculoAddOrEditModal').modal('show');                          // Muestra la ventana modal
                $('#btnUpdateVehiculo').hide();                                 // Esconde el botón ACTUALIZAR
                $('#btnAddVehiculo').show();                                    // Muestra el botón CREAR

            }

        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
    return false;
}
function ClearAllInput() {

    $('#VehiculoAddOrEditModal').modal('hide');
    $('#hfVehiculoID').val("");
    $('#selTipoVehiculo').val("");
    $('#txtMarca').val("");
    $('#txtModelo').val("");
    $('#txtFechaCompra').val("");
    $('#txtMatricula').val("");
    $('#txtTacografo').val("");
    $('#txtPma').val("");
    $('#txtTara').val("");
    $('#txtNumBastidor').val("");
    $('#txtKilometrosCompra').val("");
    $('#txtNumEjes').val("");
    $('#txtPotencia').val("");
    $('#selTaller option:selected').val("");
    $('#selCombustible').val("");
    $('#txtLongitud').val("");
    $('#txtAnchura').val("");
    $('#txtAltura').val("");
    $('#selSeguro option:selected').val("");

    $('#selTipoVehiculo').css('border-color', 'lightgrey');
    $('#txtMarca').css('border-color', 'lightgrey');
    $('#txtModelo').css('border-color', 'lightgrey');
    $('#txtFechaCompra').css('border-color', 'lightgrey');
    $('#txtMatricula').css('border-color', 'lightgrey');
    $('#txtTacografo').css('border-color', 'lightgrey');
    $('#txtPma').css('border-color', 'lightgrey');
    $('#txtTara').css('border-color', 'lightgrey');
    $('#txtNumBastidor').css('border-color', 'lightgrey');
    $('#txtKilometrosCompra').css('border-color', 'lightgrey');
    $('#txtNumEjes').css('border-color', 'lightgrey');
    $('#txtPotencia').css('border-color', 'lightgrey');
    $('#selTaller option:selected').css('border-color', 'lightgrey');
    $('#selCombustible').css('border-color', 'lightgrey');
    $('#txtLongitud').css('border-color', 'lightgrey');
    $('#txtAnchura').css('border-color', 'lightgrey');
    $('#txtAltura').css('border-color', 'lightgrey');
    $('#selSeguro option:selected').css('border-color', 'lightgrey');
}

//function for updating Vehiculo record
function UpdateVehiculo() {
    var res = ValidateUserInput();
    if (res == false) {
        return false;
    }
    var vehiculoObj = {
        ID: $('#hfVehiculoID').val(),
        TipoVehiculoID: $('#selTipoVehiculo option:selected').val(),
        Marca: $('#txtMarca').val(),
        Modelo: $('#txtModelo').val(),
        FechaCompra: $('#txtFechaCompra').val(),
        MatriculaVehiculo: $('#txtMatricula').val(),
        ModeloTacografo: $('#txtTacografo').val(),
        Pma: $('#txtPma').val(),
        Tara: $('#txtTara').val(),
        NumBastidor: $('#txtNumBastidor').val(),
        KilometrajeCompra: $('#txtKilometrosCompra').val(),
        NumEjes: $('#txtNumEjes').val(),
        PotenciaCV: $('#txtPotencia').val(),
        TallerHabitualID: $('#selTaller option:selected').val(),
        TipoCombustible: $('#selCombustible').val(),
        Longitud: $('#txtLongitud').val(),
        Anchura: $('#txtAnchura').val(),
        Altura: $('#txtAltura').val(),
        SeguroID: $('#selSeguro option:selected').val(),
    };
    if (!vehiculoObj.ID || vehiculoObj.ID <= 0) {
        alert("Invalid Id!");
        return false;
    }
    $.ajax({
        url: "/Mantenimiento/Vehiculos/UpdateVehiculo?VehiculoID=" + vehiculoObj.ID,
        data: JSON.stringify(vehiculoObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            BindVehiculosData();
            ClearAllInput();
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

function ValidateUserInput() {
    var isValid = true;
    if ($('#selTipoVehiculo').val().trim() == "") {
        $('#selTipoVehiculo').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#selTipoVehiculo').css('border-color', 'lightgrey');
    }
//    if ($('#txtMarca').val().trim() == "") {
//        $('#txtMarca').css('border-color', 'Red');
//        isValid = false;
//    }
//    else {
//        $('#txtMarca').css('border-color', 'lightgrey');
//    }
//    if ($('#txtModelo').val().trim() == "") {
//        $('#txtModelo').css('border-color', 'Red');
//        isValid = false;
//    }
//    else {
//        $('#txtModelo').css('border-color', 'lightgrey');
//    }
//    if ($('#txtFechaCompra').val().trim() == "") {
//        $('#txtFechaCompra').css('border-color', 'Red');
//        isValid = false;
//    }
//    else {
//        $('#txtFechaCompra').css('border-color', 'lightgrey');
//    }

//    if ($('#txtMatricula').val().trim() == "") {
//        $('#txtMatricula').css('border-color', 'Red');
//        isValid = false;
//    }
//    else {
//        $('#txtMatricula').css('border-color', 'lightgrey');
//    }
    return isValid;
}

//function for deleting vehiculo's record

function DeleteVehiculo(ID) {
    var ans = confirm("¿Estás seguro de borrar?");
    if (ans) {
        $.ajax({
            url: "/Mantenimiento/Vehiculos/DeleteVehiculo?VehiculoID=" + ID,
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                BindVehiculosData();
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
    }
}