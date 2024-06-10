```markdown
# ERC Token Price Updater

## Overview
This project is an ASP.NET Console application for updating the price of ERC tokens

## Prerequisites
- Visual Studio 2022 or higher
- MySQL

## Instructions

### 1. Open the Project in Visual Studio 2022 or Higher
1. Clone the repository to your local machine.
2. Open Visual Studio 2022.
3. Select **File > Open > Project/Solution**.
4. Navigate to the folder where you cloned the repository and select the solution file (.sln).

### 2. Database in MySQL
1. Use the same DB "etherscan" from the Token Manager project or create the table and seed
2. Execute the following SQL commands to create the `token` table and insert sample data:

    ```sql
    CREATE TABLE `token` (
        `id` INT(11) NOT NULL AUTO_INCREMENT,
        `symbol` VARCHAR(5) NOT NULL COLLATE 'utf8_general_ci',
        `name` VARCHAR(50) NOT NULL COLLATE 'utf8_general_ci',
        `total_supply` BIGINT(20) NOT NULL,
        `contract_address` VARCHAR(66) NOT NULL COLLATE 'utf8_general_ci',
        `total_holders` INT(11) NOT NULL,
        `price` DECIMAL(65,2) NULL DEFAULT '0.00',
        PRIMARY KEY (`id`) USING BTREE
    )
    COLLATE='utf8_general_ci'
    ENGINE=InnoDB
    ROW_FORMAT=DYNAMIC
    AUTO_INCREMENT=8;
    ```

    ```sql
    INSERT INTO `token` (`symbol`, `name`, `total_supply`, `contract_address`, `total_holders`, `price`) 
    VALUES 
    ('VEN', 'Vechain', 35987133, '0xd850942ef8811f2a866692a623011bde52a462c1', 65, 0.00),
    ('ZIR', 'Zilliqa', 53272942, '0x05f4a42e251f2d52b8ed15e9fedaacfcef1fad27', 54, 0.00),
    ('MKR', 'Maker', 45987133, '0x9f8f72aa9304c8b593d555f12ef6589cc3a579a2', 567, 0.00),
    ('BNB', 'Binance', 16579517, '0xB8c77482e45F1F44dE1745F52C74426C631bDD52', 4234234, 0.00);
    ```


### 3. Create the windows Service
1. Create the service
2. Execute the following SQL commands to create the `token` table and insert sample data:

    ```cmd
	sc create "YourServiceName" binPath= "C:\Path\To\Your\Published\Folder\YourApp.exe" DisplayName= "Your Service Display Name" start= auto
	sc description "YourServiceName" "YourServiceDescription"

    ```
3. Open Task Scheduler to run the Task every 5 minutes.
