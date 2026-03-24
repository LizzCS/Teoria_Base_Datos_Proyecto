USE sistema_bancario;
GO

CREATE PROCEDURE sp_login_usuario
    @correo nvarchar(100),
    @contrasenia nvarchar(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT usuario_id, nombre, apellido, correo_electronico
    FROM usuario
    WHERE correo_electronico = @correo
      AND contrasenia = @contrasenia
      AND estado_usuario = 1;   
    END
GO