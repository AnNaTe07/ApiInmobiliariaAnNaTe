-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 31-10-2024 a las 13:21:10
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `apiinmobiliaria`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contratos`
--

CREATE TABLE `contratos` (
  `Id` int(11) NOT NULL,
  `Desde` date NOT NULL,
  `Hasta` date NOT NULL,
  `Monto` decimal(9,1) NOT NULL,
  `InquilinoId` int(11) NOT NULL,
  `InmuebleId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contratos`
--

INSERT INTO `contratos` (`Id`, `Desde`, `Hasta`, `Monto`, `InquilinoId`, `InmuebleId`) VALUES
(1, '2024-08-01', '2025-01-24', 111111.0, 1, 1),
(2, '2024-02-01', '2026-02-11', 666666.0, 1, 6),
(3, '2023-09-13', '2025-02-02', 444444.0, 2, 2),
(4, '2024-06-04', '2026-01-01', 111222.0, 2, 3),
(5, '2024-10-01', '2024-10-31', 222333.0, 3, 4),
(6, '2024-10-11', '2024-11-13', 444555.0, 4, 5);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `Id` int(11) NOT NULL,
  `UsoInmuebleId` int(11) NOT NULL,
  `Direccion` longtext NOT NULL,
  `TipoId` int(11) NOT NULL,
  `Ambientes` int(11) NOT NULL,
  `Latitud` decimal(12,7) NOT NULL,
  `Longitud` decimal(12,7) NOT NULL,
  `Superficie` decimal(6,1) NOT NULL,
  `Precio` decimal(9,1) NOT NULL,
  `IdPropietario` int(11) NOT NULL,
  `Estado` tinyint(1) NOT NULL,
  `Foto` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`Id`, `UsoInmuebleId`, `Direccion`, `TipoId`, `Ambientes`, `Latitud`, `Longitud`, `Superficie`, `Precio`, `IdPropietario`, `Estado`, `Foto`) VALUES
(1, 1, 'Pichincha 333', 3, 2, -33.2564700, -66.2584690, 45.0, 111111.0, 2, 1, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Finmueble_1_inmueble.jpg?alt=media'),
(2, 2, 'Arenales 777', 1, 4, -33.2589650, -66.5426980, 120.0, 444444.0, 2, 1, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Fcasa4.jpg?alt=media'),
(3, 1, 'Pte Perón 432 dto \"A\"', 4, 2, -33.5421680, -66.2587410, 32.0, 111222.0, 1, 1, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Finmueble_3_inmueble.jpg?alt=media'),
(4, 2, 'Juarez 453 piso 5 dto \"3\"', 2, 3, -33.2586940, -66.3258740, 210.0, 222333.0, 2, 1, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Finmueble_4_inmueble.jpg?alt=media'),
(5, 1, 'Ruta 66 km8.5', 4, 2, -33.2569870, -66.2156980, 300.0, 444555.0, 3, 1, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Finmueble_5_inmueble.jpg?alt=media'),
(6, 2, 'Pilar 678', 6, 6, -66.5236980, -66.2136540, 500.0, 666666.0, 3, 1, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Fcasa8.png?alt=media'),
(7, 2, 'Av Dr. R. Balbín 2359', 2, 3, -33.6452980, -66.3425189, 120.0, 555555.0, 2, 1, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Finmueble_7_inmueble.jpg?alt=media'),
(8, 2, 'Avenida Siempre viva 777', 1, 5, -35.5698740, -65.3641250, 231.0, 532654.0, 2, 0, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Fcasa7.png?alt=media'),
(9, 2, 'Barrio Alegria manzana 2 casa 12', 1, 3, -44.2563250, -55.8796580, 222.0, 333222.0, 2, 0, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Fcasa4.jpg?alt=media'),
(12, 1, 'Avenida 123 ofi.7', 4, 1, -33.2564780, -66.5214780, 40.0, 60000.0, 2, 0, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Finmueble_12_inmueble.jpg?alt=media'),
(13, 1, 'Curapalihue 444', 4, 2, -33.8542360, -66.1023650, 60.0, 40000.0, 2, 0, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Finmueble_13_inmueble.jpg?alt=media'),
(14, 2, 'Bosque', 7, 5, -33.5620140, -66.8520140, 50.0, 70000.0, 2, 0, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Finmueble_14_inmueble.jpg?alt=media'),
(15, 2, 'Frente al lago', 7, 3, 75.2365470, 90.1563240, 250.0, 100000.0, 2, 0, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Finmueble_15_inmueble.jpg?alt=media'),
(16, 2, 'Aquí la casa', 1, 3, 60.5398450, 88.9654120, 120.0, 350000.0, 1, 0, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Finmueble_16_inmueble.jpg?alt=media'),
(17, 1, 'Aquí taller 987', 5, 2, -33.9754150, -69.8653150, 350.0, 500000.0, 2, 0, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Finmueble_17_inmueble.jpg?alt=media'),
(18, 2, 'Avenida 77 piso 9 depto \"A\"', 2, 3, -66.0875420, -54.9876530, 80.0, 390000.0, 2, 0, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Finmueble_18_inmueble.jpg?alt=media'),
(19, 2, 'San Martin frente al lago', 7, 3, -44.0875420, -64.0865320, 120.0, 600600.0, 2, 0, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Finmueble_19_inmueble.jpg?alt=media'),
(20, 2, 'Ruta 009 km 900', 7, 4, 90.8654200, 76.9864300, 200.0, 600000.0, 2, 0, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Finmueble_20_inmueble.jpg?alt=media'),
(85, 2, 'Av. Sueños 777 10° \"B\"', 2, 3, -99999.9999999, -99999.9999999, 80.0, 450000.0, 2, 0, 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/inmuebles%2Finmueble_85_inmueble_image.jpg?alt=media');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `Id` int(11) NOT NULL,
  `Nombre` longtext NOT NULL,
  `Apellido` longtext NOT NULL,
  `Documento` longtext NOT NULL,
  `Telefono` longtext NOT NULL,
  `Email` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`Id`, `Nombre`, `Apellido`, `Documento`, `Telefono`, `Email`) VALUES
(1, 'Esteban', 'Quito', '2265894', '2664758937', 'esteban@email.com'),
(2, 'Gallo', 'Claudio', '26598741', '2665348917', 'gallo@email.com'),
(3, 'Doña', 'Cleotilde', '11111111', '2665316598', 'cleotilde@email.com'),
(4, 'Doña', 'Florinda', '11256398', '2664335588', 'florinda@email.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `Id` int(11) NOT NULL,
  `Nro` int(11) NOT NULL,
  `Fecha` date NOT NULL,
  `Monto` decimal(9,1) NOT NULL,
  `ContratoId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pagos`
--

INSERT INTO `pagos` (`Id`, `Nro`, `Fecha`, `Monto`, `ContratoId`) VALUES
(1, 1, '2023-09-13', 444444.0, 3),
(2, 2, '2023-10-13', 444444.0, 3),
(3, 3, '2023-11-13', 444444.0, 3),
(4, 4, '2023-12-13', 444444.0, 3),
(5, 5, '2024-01-13', 444444.0, 3),
(6, 1, '2024-02-01', 666666.0, 2),
(7, 6, '2024-02-13', 444444.0, 3),
(8, 2, '2024-03-01', 666666.0, 2),
(9, 7, '2024-03-13', 444444.0, 3),
(10, 3, '2024-04-01', 666666.0, 2),
(11, 1, '2024-04-06', 111222.0, 4),
(12, 8, '2024-04-13', 444444.0, 3),
(13, 4, '2024-05-01', 666666.0, 2),
(14, 2, '2024-05-06', 111222.0, 4),
(15, 9, '2024-05-13', 444444.0, 3),
(16, 5, '2024-06-01', 666666.0, 2),
(17, 3, '2024-06-06', 111222.0, 4),
(18, 10, '2024-06-13', 444444.0, 3),
(19, 6, '2024-07-01', 666666.0, 2),
(20, 4, '2024-07-06', 111222.0, 4),
(21, 11, '2024-07-13', 444444.0, 3),
(22, 7, '2024-08-01', 666666.0, 2),
(23, 1, '2024-08-01', 111111.0, 1),
(24, 5, '2024-08-06', 111222.0, 4),
(25, 12, '2024-08-13', 444444.0, 3),
(26, 2, '2024-09-01', 111111.0, 1),
(27, 8, '2024-09-01', 666666.0, 2),
(28, 6, '2024-09-06', 111222.0, 4),
(29, 13, '2024-09-13', 444444.0, 3),
(30, 3, '2024-10-01', 111111.0, 1),
(31, 9, '2024-10-01', 666666.0, 2),
(32, 1, '2024-10-01', 222333.0, 5),
(33, 7, '2024-10-06', 111222.0, 4),
(34, 1, '2024-10-11', 444555.0, 6),
(35, 14, '2024-10-13', 444444.0, 3);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietarios`
--

CREATE TABLE `propietarios` (
  `Id` int(11) NOT NULL,
  `Apellido` longtext DEFAULT NULL,
  `Nombre` longtext DEFAULT NULL,
  `Dni` longtext DEFAULT NULL,
  `Telefono` longtext DEFAULT NULL,
  `Email` longtext DEFAULT NULL,
  `Estado` tinyint(1) NOT NULL,
  `Pass` longtext DEFAULT NULL,
  `Salt` longtext DEFAULT NULL,
  `Avatar` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`Id`, `Apellido`, `Nombre`, `Dni`, `Telefono`, `Email`, `Estado`, `Pass`, `Salt`, `Avatar`) VALUES
(1, 'Pérez', 'Juan', '12345678', '1234567890', 'juan@email.com', 1, 'ghStn4FvzSCwT0T7WzgHmfpu/4zhi7z7vETr5EYTL7s=', 'PISYZaIGoSdMNLMAy5Oalg==', NULL),
(2, 'Entesss', 'Cyndiii', '12345678', '1234567890', 'andreanataliatello@outlook.com', 1, 'O+oZ4x49kzoy43y0fRrnSXUfLASFgeKQ8msBDLuSK28=', '9GrR91hfAeY6Pcz5ncYmIA==', 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/avatars%2F2_avatar.jpg?alt=media'),
(3, 'Grillo', 'Pepe', '12345658', '123456877', 'pepe@email.com', 1, '4ZYrfI70PksuapZUtp/8KNtaTZu8PK1wlqcTtGC4m+s=', 'jUtgMk3MZNaT010pc/lzZg==', 'https://firebasestorage.googleapis.com/v0/b/appinmobiliaria-2d959.appspot.com/o/avatars%2Femogi_sin_diente.png?alt=media');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipos`
--

CREATE TABLE `tipos` (
  `Id` int(11) NOT NULL,
  `Descripcion` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tipos`
--

INSERT INTO `tipos` (`Id`, `Descripcion`) VALUES
(1, 'Casa'),
(2, 'Departamento'),
(3, 'Local'),
(4, 'Oficina'),
(5, 'Deposito'),
(6, 'Quinta'),
(7, 'Cabaña'),
(8, 'Piso');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usoinmuebles`
--

CREATE TABLE `usoinmuebles` (
  `Id` int(11) NOT NULL,
  `Nombre` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usoinmuebles`
--

INSERT INTO `usoinmuebles` (`Id`, `Nombre`) VALUES
(1, 'Comercial'),
(2, 'Residencial');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `__efmigrationshistory`
--

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20241024193409_InitialCreate', '8.0.10'),
('20241025012129_Agregar', '8.0.10');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Contratos_InmuebleId` (`InmuebleId`),
  ADD KEY `IX_Contratos_InquilinoId` (`InquilinoId`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Inmuebles_IdPropietario` (`IdPropietario`),
  ADD KEY `IX_Inmuebles_TipoId` (`TipoId`),
  ADD KEY `IX_Inmuebles_UsoInmuebleId` (`UsoInmuebleId`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Pagos_ContratoId` (`ContratoId`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `tipos`
--
ALTER TABLE `tipos`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `usoinmuebles`
--
ALTER TABLE `usoinmuebles`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contratos`
--
ALTER TABLE `contratos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=86;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=36;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `tipos`
--
ALTER TABLE `tipos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT de la tabla `usoinmuebles`
--
ALTER TABLE `usoinmuebles`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `FK_Contratos_Inmuebles_InmuebleId` FOREIGN KEY (`InmuebleId`) REFERENCES `inmuebles` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_Contratos_Inquilinos_InquilinoId` FOREIGN KEY (`InquilinoId`) REFERENCES `inquilinos` (`Id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `FK_Inmuebles_Propietarios_IdPropietario` FOREIGN KEY (`IdPropietario`) REFERENCES `propietarios` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_Inmuebles_Tipos_TipoId` FOREIGN KEY (`TipoId`) REFERENCES `tipos` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_Inmuebles_UsoInmuebles_UsoInmuebleId` FOREIGN KEY (`UsoInmuebleId`) REFERENCES `usoinmuebles` (`Id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `FK_Pagos_Contratos_ContratoId` FOREIGN KEY (`ContratoId`) REFERENCES `contratos` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
