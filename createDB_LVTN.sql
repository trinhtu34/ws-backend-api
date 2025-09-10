CREATE DATABASE IF NOT EXISTS dbluanvan2
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE dbluanvan2;

CREATE TABLE roles (
    roles_id INT NOT NULL,
    roles_description VARCHAR(100),
    PRIMARY KEY (roles_id)
) DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE users (
    user_id VARCHAR(20) NOT NULL,
    u_password VARCHAR(50),
    customer_name VARCHAR(10),
    roles_id INT NOT NULL,
    phone_number VARCHAR(15),
    email VARCHAR(100),
    address VARCHAR(500),
    createAt DATETIME DEFAULT CURRENT_TIMESTAMP(),
    PRIMARY KEY (user_id)
) DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE region (
    region_id INT NOT NULL AUTO_INCREMENT,
    region_name VARCHAR(50) CHARACTER SET utf8mb4 NOT NULL,
    PRIMARY KEY (region_id)
) DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE category (
    category_id INT NOT NULL AUTO_INCREMENT,
    category_name VARCHAR(50) CHARACTER SET utf8mb4 NOT NULL,
    PRIMARY KEY (category_id)
) DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE menu (
    dish_id VARCHAR(5) NOT NULL,
    dish_name VARCHAR(255) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    descriptions TEXT, 
    isAvailable TINYINT(1) DEFAULT 0,
    category_id INT NOT NULL,
    region_id INT NOT NULL,
    images VARCHAR(255),
    PRIMARY KEY (dish_id)
) DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE tables (
    table_id INT NOT NULL AUTO_INCREMENT,
    capacity INT,
    deposit DECIMAL(10,2),
    description TEXT,
    region_id INT NOT NULL,
    PRIMARY KEY (table_id)
) DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE contact_form (
    contact_id INT NOT NULL AUTO_INCREMENT,
    user_id VARCHAR(20) NOT NULL,
    content TEXT NOT NULL,
    createAt DATETIME DEFAULT CURRENT_TIMESTAMP(),
    PRIMARY KEY (contact_id)
) DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE orderTables (
    orderTableId BIGINT NOT NULL AUTO_INCREMENT,
    user_id VARCHAR(20),
    starting_time DATETIME NOT NULL,
    isCancel TINYINT(1) NULL,
    total_price DECIMAL(10,2),
    total_deposit DECIMAL(10,2),
    orderDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (orderTableId)
) DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE orderTablesDetails (
    orderTablesDetailsId INT NOT NULL AUTO_INCREMENT,
    orderTableId BIGINT,
    table_id INT,
    PRIMARY KEY (orderTablesDetailsId)
) DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE orderFoodDetails (
    orderFoodDetailsId INT NOT NULL AUTO_INCREMENT,
    orderTableId BIGINT NOT NULL,
    dish_id VARCHAR(5) NOT NULL,
    quantity INT NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    note TEXT,
    PRIMARY KEY (orderFoodDetailsId)
) DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE Cart (
    cart_id BIGINT NOT NULL AUTO_INCREMENT,
    user_id VARCHAR(20) NOT NULL,
    order_time DATETIME,
	isCancel TINYINT(1) NULL,
    isFinish BOOLEAN DEFAULT FALSE,
    totalPrice decimal(10,2),
    PRIMARY KEY (cart_id)
) DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE cart_details (
    cart_details_id INT NOT NULL AUTO_INCREMENT,
    cart_id BIGINT,
    dish_id VARCHAR(5) NOT NULL,
    quantity INT DEFAULT 0,
    price DECIMAL(10,2),
    PRIMARY KEY (cart_details_id)
) DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE PaymentResults (
    PaymentResultId BIGINT PRIMARY KEY AUTO_INCREMENT,
    orderTableId BIGINT NULL,
    cart_id BIGINT NULL,
    Amount decimal(10,2),
    PaymentId BIGINT NULL,
    IsSuccess TINYINT(1) NULL,
    Description VARCHAR(255),
    Timestamp DATETIME NULL,
    VnpayTransactionId BIGINT,
    PaymentMethod VARCHAR(100),
    BankCode VARCHAR(20),
    BankTransactionId VARCHAR(50),
    ResponseDescription VARCHAR(255),
    TransactionStatusDescription VARCHAR(255)
) DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Foreign key constraints
ALTER TABLE orderTables
ADD CONSTRAINT fk_ordertables_users FOREIGN KEY (user_id) REFERENCES users(user_id);

ALTER TABLE users 
ADD CONSTRAINT fk_users_roles FOREIGN KEY (roles_id) REFERENCES roles(roles_id);

ALTER TABLE contact_form 
ADD CONSTRAINT fk_contact_form_user FOREIGN KEY (user_id) REFERENCES users(user_id);

ALTER TABLE menu 
ADD CONSTRAINT fk_menu_category FOREIGN KEY (category_id) REFERENCES category(category_id),
ADD CONSTRAINT fk_menu_region FOREIGN KEY (region_id) REFERENCES region(region_id);

ALTER TABLE tables 
ADD CONSTRAINT fk_tables_region FOREIGN KEY (region_id) REFERENCES region(region_id);

ALTER TABLE orderTablesDetails 
ADD CONSTRAINT fk_orderTablesDetails_order FOREIGN KEY (orderTableId) REFERENCES orderTables(orderTableId),
ADD CONSTRAINT fk_orderTablesDetails_table FOREIGN KEY (table_id) REFERENCES tables(table_id);

ALTER TABLE orderFoodDetails 
ADD CONSTRAINT fk_orderFoodDetails_order FOREIGN KEY (orderTableId) REFERENCES orderTables(orderTableId),
ADD CONSTRAINT fk_orderFoodDetails_menu FOREIGN KEY (dish_id) REFERENCES menu(dish_id);

ALTER TABLE PaymentResults 
ADD CONSTRAINT fk_PaymentResults_order FOREIGN KEY (orderTableId) REFERENCES orderTables(orderTableId);
ALTER TABLE PaymentResults 
ADD CONSTRAINT fk_PaymentResults_cart FOREIGN KEY (cart_id) REFERENCES Cart(cart_id);

ALTER TABLE Cart 
ADD CONSTRAINT fk_cart_user FOREIGN KEY (user_id) REFERENCES users(user_id);

ALTER TABLE cart_details 
ADD CONSTRAINT fk_cart_details_order FOREIGN KEY (cart_id) REFERENCES Cart(cart_id),
ADD CONSTRAINT fk_cart_details_menu FOREIGN KEY (dish_id) REFERENCES menu(dish_id);

insert into roles() values (1 ,'this role used by admin');
insert into roles() values (0 ,'this role used by customer');