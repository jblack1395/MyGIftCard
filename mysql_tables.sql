DROP DATABASE mygiftcard;
CREATE DATABASE mygiftcard;
USE mygiftcard;

CREATE TABLE IF NOT EXISTS `customers` (
 `customerNumber` int(11) NOT NULL AUTO_INCREMENT,
 `customerName` varchar(50) NOT NULL,
`password` varchar(24) NOT NULL,
`logoUploaded` int(1),
 PRIMARY KEY (`customerNumber`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=172 ;

CREATE TABLE IF NOT EXISTS `contact_info` (
   `contactInfoNumber` int(11) NOT NULL AUTO_INCREMENT,
   `email` varchar(50) NOT NULL,
   `phone` varchar(50) NOT NULL,
   `fax` varchar(50) NOT NULL,
   `website` varchar(50) NOT NULL,
   `mastercardAccepted` int(1),
   `discoverAccepted` int(1),
   `americanExpressAccepted` int(1),
   `visaAccepted` int(1),
   `allowGratuity` int(1),
   `allowMailOption` int(1),
   `shippingCost` double,
   `expireAfterDays` int(16),
   `finePrint` varchar(512),
`customerid` int(11) NOT NULL,
  PRIMARY KEY(`contactInfoNumber`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=172 ;

CREATE TABLE IF NOT EXISTS `address` (
   `addressNumber` int(11) NOT NULL AUTO_INCREMENT,
 `address` varchar(50) NOT NULL,
 `city` varchar(50) NOT NULL,
 `state` varchar(50) DEFAULT NULL,
 `postalCode` varchar(15) DEFAULT NULL,
 `country` varchar(50) NOT NULL,
`customerid` int(11) NOT NULL,
  PRIMARY KEY(`addressNumber`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=172 ;

CREATE TABLE IF NOT EXISTS `giftcards` (
`giftcardNumber` int(11) NOT NULL AUTO_INCREMENT,
`giftcardMessage` varchar(200) NOT NULL,
`giftcardRecipient` varchar(50) NOT NULL,
`customerid` int(11) NOT NULL,
PRIMARY KEY (`giftcardNumber`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=172 ;

ALTER TABLE giftcards
ADD CONSTRAINT FK_giftcards_customer
FOREIGN KEY (customerid) REFERENCES customers(customerNumber) 
ON UPDATE CASCADE
ON DELETE CASCADE;

ALTER TABLE contact_info
ADD CONSTRAINT FK_contactInfo_customer 
FOREIGN KEY (customerid) REFERENCES customers(customerNumber) 
ON UPDATE CASCADE
ON DELETE CASCADE;

ALTER TABLE address
ADD CONSTRAINT FK_address_customer 
FOREIGN KEY (customerid) REFERENCES customers(customerNumber) 
ON UPDATE CASCADE
ON DELETE CASCADE;
