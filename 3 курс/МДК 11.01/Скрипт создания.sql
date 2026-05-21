-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema KontoraGuard
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema KontoraGuard
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `KontoraGuard` DEFAULT CHARACTER SET cp1251 ;
USE `KontoraGuard` ;

-- -----------------------------------------------------
-- Table `KontoraGuard`.`k_staff`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `KontoraGuard`.`k_staff` (
  `staff_num` INT GENERATED ALWAYS AS () VIRTUAL,
  `staf_name` VARCHAR(45) NOT NULL,
  `staff_post` VARCHAR(45) NULL,
  `staff_hiredate` DATE NULL,
  `staff_terndate` DATE NULL,
  `k_staffcol` VARCHAR(45) NULL,
  `k_dept_dept_num` INT NOT NULL,
  PRIMARY KEY (`staff_num`),
  INDEX `fk_k_staff_k_dept_idx` (`k_dept_dept_num` ASC) VISIBLE,
  CONSTRAINT `fk_k_staff_k_dept`
    FOREIGN KEY (`k_dept_dept_num`)
    REFERENCES `KontoraGuard`.`k_dept` (`dept_num`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `KontoraGuard`.`k_dept`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `KontoraGuard`.`k_dept` (
  `dept_num` INT GENERATED ALWAYS AS () VIRTUAL,
  `dept_full_name` VARCHAR(45) NULL,
  `dept_short_name` VARCHAR(10) NOT NULL,
  `k_staff_staff_num` INT NOT NULL,
  PRIMARY KEY (`dept_num`),
  INDEX `fk_k_dept_k_staff1_idx` (`k_staff_staff_num` ASC) VISIBLE,
  CONSTRAINT `fk_k_dept_k_staff1`
    FOREIGN KEY (`k_staff_staff_num`)
    REFERENCES `KontoraGuard`.`k_staff` (`staff_num`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `KontoraGuard`.`k_firm`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `KontoraGuard`.`k_firm` (
  `firm_num` INT GENERATED ALWAYS AS () VIRTUAL,
  `firm_name` VARCHAR(45) NOT NULL,
  `firm_addr` VARCHAR(45) NULL,
  `firm_phone` VARCHAR(20) NULL,
  PRIMARY KEY (`firm_num`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `KontoraGuard`.`k_contract`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `KontoraGuard`.`k_contract` (
  `contract_num` INT GENERATED ALWAYS AS () VIRTUAL,
  `contract_date` DATE NOT NULL,
  `contract_type` ENUM('A', 'B', 'C') NOT NULL,
  `k_firm_firm_num` INT NOT NULL,
  `k_staff_staff_num` INT NOT NULL,
  PRIMARY KEY (`contract_num`),
  INDEX `fk_k_contract_k_firm1_idx` (`k_firm_firm_num` ASC) VISIBLE,
  INDEX `fk_k_contract_k_staff1_idx` (`k_staff_staff_num` ASC) VISIBLE,
  CONSTRAINT `fk_k_contract_k_firm1`
    FOREIGN KEY (`k_firm_firm_num`)
    REFERENCES `KontoraGuard`.`k_firm` (`firm_num`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_k_contract_k_staff1`
    FOREIGN KEY (`k_staff_staff_num`)
    REFERENCES `KontoraGuard`.`k_staff` (`staff_num`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `KontoraGuard`.`k_bill`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `KontoraGuard`.`k_bill` (
  `bill_num` INT GENERATED ALWAYS AS () VIRTUAL,
  `bill_date` DATE NULL,
  `bill_sum` DECIMAL(9,2) NULL,
  `bill_tern` DATE NULL,
  `bill_peni` DECIMAL(6,2) NULL,
  `k_contract_contract_num` INT NOT NULL,
  PRIMARY KEY (`bill_num`),
  INDEX `fk_k_bill_k_contract1_idx` (`k_contract_contract_num` ASC) VISIBLE,
  CONSTRAINT `fk_k_bill_k_contract1`
    FOREIGN KEY (`k_contract_contract_num`)
    REFERENCES `KontoraGuard`.`k_contract` (`contract_num`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `KontoraGuard`.`k_payment`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `KontoraGuard`.`k_payment` (
  `payment_num` INT GENERATED ALWAYS AS () VIRTUAL,
  `payment_date` DATE NULL,
  `payment_sum` DECIMAL(9,2) NULL,
  `k_bill_bill_num` INT NOT NULL,
  PRIMARY KEY (`payment_num`),
  INDEX `fk_k_payment_k_bill1_idx` (`k_bill_bill_num` ASC) VISIBLE,
  CONSTRAINT `fk_k_payment_k_bill1`
    FOREIGN KEY (`k_bill_bill_num`)
    REFERENCES `KontoraGuard`.`k_bill` (`bill_num`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `KontoraGuard`.`k_price`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `KontoraGuard`.`k_price` (
  `price_num` INT NOT NULL,
  `price_name` VARCHAR(45) NOT NULL,
  `price_sum` DECIMAL(9,2) NULL,
  `price_type` VARCHAR(1) NULL,
  PRIMARY KEY (`price_num`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `KontoraGuard`.`k_protokol`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `KontoraGuard`.`k_protokol` (
  `k_price_price_num` INT NOT NULL,
  `kolvo` INT NOT NULL,
  `price_sum` DECIMAL(9,2) NOT NULL,
  `k_bill_bill_num` INT NOT NULL,
  PRIMARY KEY (`k_price_price_num`, `k_bill_bill_num`),
  INDEX `fk_k_payment_has_k_price_k_price1_idx` (`k_price_price_num` ASC) VISIBLE,
  INDEX `fk_k_protokol_k_bill1_idx` (`k_bill_bill_num` ASC) VISIBLE,
  CONSTRAINT `fk_k_payment_has_k_price_k_price1`
    FOREIGN KEY (`k_price_price_num`)
    REFERENCES `KontoraGuard`.`k_price` (`price_num`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_k_protokol_k_bill1`
    FOREIGN KEY (`k_bill_bill_num`)
    REFERENCES `KontoraGuard`.`k_bill` (`bill_num`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
