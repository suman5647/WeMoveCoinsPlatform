GO 
  
IF EXISTS (SELECT name FROM sys.indexes  
            WHERE name = N'IX_AuditTrail_Created')   
    DROP INDEX IX_AuditTrail_Created ON AuditTrail;   
GO  
 
CREATE NONCLUSTERED INDEX IX_AuditTrail_Created 
    ON [AuditTrail]([Created] ASC);   
GO  


GO 
  
IF EXISTS (SELECT name FROM sys.indexes  
            WHERE name = N'IX_AuditTrail_Level')   
    DROP INDEX IX_AuditTrail_Level ON AuditTrail;   
GO  
 
CREATE NONCLUSTERED INDEX IX_AuditTrail_Level
    ON [AuditTrail]([AuditTrailLevelId] ASC);   
GO  


GO 
  
IF EXISTS (SELECT name FROM sys.indexes  
            WHERE name = N'IX_AuditTrail_Status')   
    DROP INDEX IX_AuditTrail_Status ON AuditTrail;   
GO  
 
CREATE NONCLUSTERED INDEX IX_AuditTrail_Status
    ON [AuditTrail]([Status] ASC);   
GO  

