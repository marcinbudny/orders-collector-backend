CREATE SCHEMA IF NOT EXISTS reads;

CREATE TABLE IF NOT EXISTS reads."order"
(
    id character varying NOT NULL,
    date date NOT NULL,
    local_id character varying NOT NULL,
    responsible_person character varying NULL,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS reads."order_item"
(
    id serial NOT NULL,
    person_name character varying NOT NULL,
    item_name character varying NOT NULL,
    added_at timestamp without time zone NOT NULL,
    order_id character varying NOT NULL,
    PRIMARY KEY (id)
); 