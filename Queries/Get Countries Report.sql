select C.[Name], * 
from dbo.countries C
inner join dbo.ipaddresses I on C.Id = I.countryId




with
LastUpdateAt as (
  select [IP], max(UpdatedAt) as LastUpdateAt
  from dbo.ipaddresses I
  Group by I.[IP]
)
select C.[Name], count(I.[IP]) AddressesCount, lua.LastUpdateAt
from dbo.ipaddresses I
inner join dbo.Countries C on C.Id = I.CountryId
inner join LastUpdateAt lua
  on lua.[IP] = I.[IP]
  and lua.LastUpdateAt = I.UpdatedAt
  group by C.[Name], lua.LastUpdateAt

  with
                        LastUpdateAt as (
                          select [IP], max(UpdatedAt) as LastUpdateAt
                          from dbo.ipaddresses I
                          Group by I.[IP]
                        )
                        select C.[Name], count(I.[IP]) AddressesCount, lua.LastUpdateAt
                            from dbo.ipaddresses I
                            inner join dbo.Countries C on C.Id = I.CountryId
                            inner join LastUpdateAt lua
                              on lua.[IP] = I.[IP]
                              and lua.LastUpdateAt = I.UpdatedAt
                              group by C.[Name], lua.LastUpdateAt