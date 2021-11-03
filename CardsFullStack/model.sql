create database CardGame;

use CardGame;

create table Deck (
    deck_id varchar(20),
    username varchar(20),
    created_at datetime,
    primary key (deck_id)
);

create table Card (
	id int not null auto_increment,
    deck_id varchar(20),
    image varchar(200),
    card_code char(2),
    username varchar(20),
    created_at datetime,
    primary key (id)
);