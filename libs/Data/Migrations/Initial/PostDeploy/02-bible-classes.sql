SET @ParentId = 2;
SET @CreatedOn = UTC_TIMESTAMP();
SET @UpdatedOn = UTC_TIMESTAMP();
SET @mp3 = 'audio/mpeg';
set @Path = '/talks/Bible Classes/'

INSERT INTO Items (
    Path
    , Name
    , Author
    , Description
    , ParentId
    , IsFolder
    , ContentType
    , PublishedOn
    , CreatedOn
    , UpdatedOn
) VALUES (
    @Path + '200827_000.mp3'
    , 'All in All'
    , 'Mike McStravick'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-08-27'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + '200820_000.mp3'
    , 'Time Would Fail Me to Tell of... David - Part 2'
    , 'Nathan Crawford'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-08-20'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + '200813_000.mp3'
    , 'John 1'
    , 'Micah Quindazzi'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-08-13'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + '200806_000.mp3'
    , 'Romans'
    , 'Clyde Snobelen'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-08-06'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + '200716_000.mp3'
    , 'All in All'
    , 'Mike McStravick'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-07-16'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + ''
    , 'name'
    , 'author'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-09-27'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + ''
    , 'name'
    , 'author'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-09-27'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + ''
    , 'name'
    , 'author'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-09-27'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + ''
    , 'name'
    , 'author'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-09-27'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + ''
    , 'name'
    , 'author'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-09-27'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + ''
    , 'name'
    , 'author'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-09-27'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + ''
    , 'name'
    , 'author'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-09-27'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + ''
    , 'name'
    , 'author'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-09-27'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + ''
    , 'name'
    , 'author'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-09-27'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + ''
    , 'name'
    , 'author'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-09-27'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + ''
    , 'name'
    , 'author'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-09-27'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + ''
    , 'name'
    , 'author'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-09-27'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + ''
    , 'name'
    , 'author'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-09-27'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + ''
    , 'name'
    , 'author'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-09-27'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + ''
    , 'name'
    , 'author'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-09-27'
    , @CreatedOn
    , @UpdatedOn
)
, (
    @Path + ''
    , 'name'
    , 'author'
    , null
    , @ParentId
    , 0
    , @mp3
    , '2020-09-27'
    , @CreatedOn
    , @UpdatedOn
)