INSERT INTO Items (
    Id
    , Path
    , Name
    , Author
    , Description
    , IsFolder
    , SortOrder
    , PublishedOn
    , CreatedOn
    , UpdatedOn
) VALUES (
    1
    , '/talks/Exhortations'
    , 'Exhortations'
    , ''
    , ''
    , 1
    , 1
    , null
    , UTC_TIMESTAMP()
    , UTC_TIMESTAMP()
)
, (
    2
    , '/talks/Bible Talks'
    , 'Bible Talks'
    , ''
    , ''
    , 1
    , 2
    , null
    , UTC_TIMESTAMP()
    , UTC_TIMESTAMP()
)
, (
    3
    , '/talks/Bible Classes'
    , 'Bible Classes'
    , ''
    , ''
    , 1
    , 3
    , null
    , UTC_TIMESTAMP()
    , UTC_TIMESTAMP()
)
, (
    4
    , '/talks/Study Days'
    , 'Study Days'
    , ''
    , ''
    , 1
    , 4
    , null
    , UTC_TIMESTAMP()
    , UTC_TIMESTAMP()
)

ALTER TABLE Items AUTO_INCREMENT = 5;