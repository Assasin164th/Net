CREATE TABLE `Туристы` (
  `id_туриста` INT AUTO_INCREMENT,
  `фио` VARCHAR(150) NOT NULL,
  `паспортные_данные` VARCHAR(50) NOT NULL,
  `пол` VARCHAR(10),
  `возраст` INT,
  `дети` VARCHAR(10) DEFAULT 'Нет',
  PRIMARY KEY (`id_туриста`)
) ENGINE=InnoDB;

CREATE TABLE `Туры` (
  `id_тура` INT AUTO_INCREMENT,
  `название` VARCHAR(100) NOT NULL,
  `страна` VARCHAR(50) NOT NULL,
  `города` VARCHAR(150),
  `тип_передвижения` VARCHAR(50),
  `тип_питания` VARCHAR(50),
  `цена_тура` DECIMAL(10,2),
  `тип_проживания` VARCHAR(50),
  PRIMARY KEY (`id_тура`)
) ENGINE=InnoDB;

CREATE TABLE `Гостиницы` (
  `id_гостиницы` INT AUTO_INCREMENT,
  `название_гостиницы` VARCHAR(100) NOT NULL,
  `страна` VARCHAR(50) NOT NULL,
  `город` VARCHAR(50) NOT NULL,
  `адрес` VARCHAR(150),
  `кол_во_мест` INT,
  `тип_гостиницы` VARCHAR(50),
  PRIMARY KEY (`id_гостиницы`)
) ENGINE=InnoDB;

CREATE TABLE `Туристическая_группа` (
  `id_группы` INT AUTO_INCREMENT,
  `название` VARCHAR(100) NOT NULL,
  `дата_отправления` DATE NOT NULL,
  `дата_прибытия` DATE NOT NULL,
  `id_тура` INT NOT NULL,
  `кол_во_туристов` INT,
  PRIMARY KEY (`id_группы`),
  INDEX `fk_группа_тур_idx` (`id_тура`),
  CONSTRAINT `fk_группа_тур` FOREIGN KEY (`id_тура`) REFERENCES `Туры`(`id_тура`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE `Состав_групп` (
  `id_записи` INT AUTO_INCREMENT,
  `дата_продажи` DATE NOT NULL,
  `id_туриста` INT NOT NULL,
  `id_группы` INT NOT NULL,
  `цена_билета` DECIMAL(10,2),
  PRIMARY KEY (`id_записи`),
  INDEX `fk_состав_турист_idx` (`id_туриста`),
  INDEX `fk_состав_группа_idx` (`id_группы`),
  CONSTRAINT `fk_состав_турист` FOREIGN KEY (`id_туриста`) REFERENCES `Туристы`(`id_туриста`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_состав_группа` FOREIGN KEY (`id_группы`) REFERENCES `Туристическая_группа`(`id_группы`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE `Ведомость_продаж` (
  `id_ведомости` INT AUTO_INCREMENT,
  `дата` DATE NOT NULL,
  `id_группы` INT NOT NULL,
  `id_гостиницы` INT NOT NULL,
  `общая_стоимость` DECIMAL(12,2),
  PRIMARY KEY (`id_ведомости`),
  INDEX `fk_ведомость_группа_idx` (`id_группы`),
  INDEX `fk_ведомость_гостиница_idx` (`id_гостиницы`),
  CONSTRAINT `fk_ведомость_группа` FOREIGN KEY (`id_группы`) REFERENCES `Туристическая_группа`(`id_группы`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_ведомость_гостиница` FOREIGN KEY (`id_гостиницы`) REFERENCES `Гостиницы`(`id_гостиницы`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

SET FOREIGN_KEY_CHECKS = 1;