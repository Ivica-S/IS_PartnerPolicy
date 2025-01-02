using Dapper;
using System.Data;
using IS_PartnerPolicy.Models;
using Microsoft.Data.SqlClient;
using IS_PartnerPolicy.DTO;

namespace IS_PartnerPolicy.Repository
{
    public class PartnerRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<PartnerRepository> _logger;

        public PartnerRepository(string connectionString, ILogger<PartnerRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        // Funkcija za dohvat svih partnera
        public IEnumerable<Partner> GetAllPartners()
        {
            try
            {
                using (IDbConnection dbConnection = new SqlConnection(_connectionString))
                {
                    dbConnection.Open();

                    // SQL upit s LEFT JOIN koji dohvaća partnere i njihove police
                    var query = @"
            SELECT p.*, 
                   pol.PolicaId, pol.PolicyNumber, pol.Amount
            FROM Partner p
            LEFT JOIN PartnerPolica pol ON p.PartnerId = pol.PartnerId
            ORDER BY p.PartnerId DESC";

                    // Dictionary za mapiranje partnera i njihovih polica
                    var partnerDictionary = new Dictionary<int, Partner>();

                    // Dohvat partnera i njihovih polica
                    var partners = dbConnection.Query<Partner, PartnerPolica, Partner>(
                        query,
                        (p, pol) =>
                        {
                            // Provjeravamo je li partner već dodan u dictionary
                            if (!partnerDictionary.TryGetValue(p.PartnerId, out var partnerEntry))
                            {
                                partnerEntry = p;
                                partnerEntry.Policys = new List<PartnerPolica>();  // Inicijaliziraj listu polica
                                partnerDictionary.Add(p.PartnerId, partnerEntry);
                            }

                            // Dodajemo policu ako postoji
                            if (pol != null)
                            {
                                partnerEntry.Policys.Add(pol);
                            }

                            return partnerEntry;
                        },
                        splitOn: "PolicaId"
                    ).Distinct().ToList(); // Distinct osigurava da se partneri ne ponavljaju

                    return partners;
                }
            }
            catch (Exception ex)
            {
                // Logiranje greške
                _logger.LogError(ex, "Došlo je do pogreške prilikom dohvaćanja partnera i njihovih polica iz baze.");
                throw; // baci iznimku da je kontrolira viši sloj (npr. API kontroler)
            }
        }


        public int AddPartner(PartnerCreateDto partner)
        {
            try
            {
                using (IDbConnection dbConnection = new SqlConnection(_connectionString))
                {
                    dbConnection.Open();
                    var query = @"
                    INSERT INTO Partner 
                        (FirstName, LastName, Address, PartnerNumber, PartnerTypeId, CroatianPIN, CreatedByUser, IsForeign, ExternalCode, Gender) 
                    VALUES 
                        (@FirstName, @LastName, @Address, @PartnerNumber, @PartnerTypeId, @CroatianPIN, @CreatedByUser, @IsForeign, @ExternalCode, @Gender);
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";

                    // Izvrši upit i dobiti generirani ID
                    int newPartnerId = dbConnection.QuerySingle<int>(query, partner);

                    // Vrati ID
                    return newPartnerId;
                }
            }
            catch (SqlException ex)
            {
                // Logiranje SQL greške
                _logger.LogError(ex, "Došlo je do pogreške pri dodavanju novog partnera u bazu.");
                throw; //  bacanje iznimke prema višem sloju (kontroler)
            }
            catch (Exception ex)
            {
                // Logiranje drugih grešaka
                _logger.LogError(ex, "Došlo je do nepoznate greške pri dodavanju novog partnera.");
                throw; //  bacanje iznimke prema višem sloju (kontroler)
            }
        }

        public int EditPartner(EditPartnerDTO partner)
        {
            try
            {
                using (IDbConnection dbConnection = new SqlConnection(_connectionString))
                {
                    dbConnection.Open();

                    // Provjeri postoji li partner prije nego što izvršiš UPDATE
                    var partnerExists = dbConnection.ExecuteScalar<bool>(
                        "SELECT COUNT(1) FROM Partner WHERE PartnerId = @PartnerId",
                        new { PartnerId = partner.PartnerId }
                    );

                    if (!partnerExists)
                    {
                        _logger.LogWarning("Partner s ID: {PartnerId} nije pronađen.", partner.PartnerId);
                        throw new Exception("Partner s navedenim ID-om ne postoji.");
                    }

                    // SQL upit za ažuriranje
                    var query = @"
            UPDATE Partner
            SET 
                FirstName = @FirstName, 
                LastName = @LastName, 
                Address = @Address, 
                PartnerNumber = @PartnerNumber, 
                PartnerTypeId = @PartnerTypeId, 
                CroatianPIN = @CroatianPIN, 
                CreatedByUser = @CreatedByUser, 
                IsForeign = @IsForeign, 
                ExternalCode = @ExternalCode, 
                Gender = @Gender
            WHERE PartnerId = @PartnerId;";


                    int rowsAffected = dbConnection.Execute(query, partner);

                    // Provjeri da li je doista ažuriran barem jedan red
                    if (rowsAffected == 0)
                    {
                        _logger.LogWarning("Ažuriranje partnera s ID: {PartnerId} nije donijelo promjene.", partner.PartnerId);
                        throw new Exception("Nema promjena za ažuriranje. Provjerite podatke.");
                    }

                    // Logiraj uspješan završetak operacije
                    _logger.LogInformation("Partner s ID: {PartnerId} uspješno ažuriran. Broj ažuriranih redaka: {RowsAffected}", partner.PartnerId, rowsAffected);

                    return rowsAffected;  // Vraća broj ažuriranih redaka (može biti 1 ako je sve prošlo dobro)
                }
            }
            catch (SqlException ex)
            {
                // Logiranje SQL greške
                _logger.LogError(ex, "SQL pogreška prilikom ažuriranja partnera s ID: {PartnerId}.", partner.PartnerId);
                throw;  //  bacanje iznimke prema višem sloju
            }
            catch (Exception ex)
            {
                // Logiranje drugih grešaka
                _logger.LogError(ex, "Došlo je do pogreške pri ažuriranju partnera s ID: {PartnerId}.", partner.PartnerId);
                throw;  //  bacanje iznimke prema višem sloju
            }
        }


        // Funkcija za dohvat partnera prema ID-u
        public Partner GetPartnerById(int id)
        {
            try
            {
                using (IDbConnection dbConnection = new SqlConnection(_connectionString))
                {
                    dbConnection.Open();

                    // SQL upit koji koristi JOIN da bi povezao Partner i njegove Polica
                    var query = @"
            SELECT p.*, 
                   pol.PolicaId, pol.PolicyNumber, pol.Amount
            FROM Partner p
            LEFT JOIN PartnerPolica pol ON p.PartnerId = pol.PartnerId
            WHERE p.PartnerId = @Id";

                    // Dohvati partnera i njegove police
                    var partnerDictionary = new Dictionary<int, Partner>();
                    var partner = dbConnection.Query<Partner, PartnerPolica, Partner>(
                        query,
                        (p, pol) =>
                        {
                            // Ako partner nije još dodan u dictionary, dodaj ga
                            if (!partnerDictionary.TryGetValue(p.PartnerId, out var partnerEntry))
                            {
                                partnerEntry = p;
                                partnerEntry.Policys = new List<PartnerPolica>();
                                partnerDictionary.Add(p.PartnerId, partnerEntry);
                            }

                            // Dodaj policu partneru
                            if (pol != null)
                            {
                                partnerEntry.Policys.Add(pol);
                            }

                            return partnerEntry;
                        },
                        new { Id = id },
                        splitOn: "PolicaId"
                    ).FirstOrDefault();

                    return partner;
                }
            }
            catch (SqlException ex)
            {
                // Logiranje SQL greške
                _logger.LogError(ex, "SQL pogreška prilikom dohvaćanja partnera s ID: {PartnerId}.", id);
                throw;  //  bacanje iznimke prema višem sloju
            }
            catch (Exception ex)
            {
                // Logiranje drugih grešaka
                _logger.LogError(ex, "Došlo je do pogreške pri dohvaćanju partnera s ID: {PartnerId}.", id);
                throw;  //  bacanje iznimke prema višem sloju
            }
        }

        public Partner GetPartnerWithPolicies(int partnerId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // SQL upit za dohvat partnera i njihovih polica
                    var query = @"
                SELECT p.PolicaId, p.PartnerId, p.PolicyNumber, p.Amount, 
                        pr.PartnerId, pr.FirstName, pr.LastName
                FROM PartnerPolica p
                RIGHT JOIN Partner pr ON p.PartnerId = pr.PartnerId
                WHERE pr.PartnerId = @PartnerId
                ORDER BY p.PolicaId DESC";

                    var partnerDictionary = new Dictionary<int, Partner>();

                    var result = connection.Query<PartnerPolica, Partner, PartnerPolica>(
                        query,
                        (polica, partner) =>
                        {
                            if (!partnerDictionary.TryGetValue(partner.PartnerId, out var partnerEntry))
                            {
                                partnerEntry = partner;
                                partnerEntry.Policys = new List<PartnerPolica>();
                                partnerDictionary.Add(partner.PartnerId, partnerEntry);
                            }

                            // Provjera da polica nije NULL (t.j. postoji)
                            if (polica.PolicaId != 0) // Ako je PolicaId 0, znači da polica ne postoji
                            {
                                partnerEntry.Policys.Add(polica);
                            }
                            return polica;
                        },
                        new { PartnerId = partnerId },
                        splitOn: "PartnerId"
                    ).ToList();

                    return partnerDictionary.Values.FirstOrDefault();
                }
            }
            catch (SqlException ex)
            {
                // Logiranje SQL greške
                _logger.LogError(ex, "SQL pogreška prilikom dohvaćanja partnera s policama ID: {PartnerId}.", partnerId);
                throw;  //  bacanje iznimke prema višem sloju
            }
            catch (Exception ex)
            {
                // Logiranje drugih grešaka
                _logger.LogError(ex, "Došlo je do pogreške pri dohvaćanju partnera s policama s ID: {PartnerId}.", partnerId);
                throw;  //  bacanje iznimke prema višem sloju
            }
        }

        public Partner AddPartnerPolicy(int partnerId, string policyNumber, decimal amount)
        {
            try
            {
                // Dodaj novu policu u bazu
                var query = "INSERT INTO PartnerPolica (PartnerId, PolicyNumber, Amount) VALUES (@PartnerId, @PolicyNumber, @Amount)";

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Execute(query, new { PartnerId = partnerId, PolicyNumber = policyNumber, Amount = amount });
                }

                return GetPartnerWithPolicies(partnerId);
            }
            catch (SqlException ex)
            {
                // Logiranje SQL greške
                _logger.LogError(ex, "SQL pogreška prilikom dodavanja nove police za partnera s ID: {PartnerId}.", partnerId);
                throw;  //  bacanje iznimke prema višem sloju
            }
            catch (Exception ex)
            {
                // Logiranje drugih grešaka
                _logger.LogError(ex, "Došlo je do pogreške pri dodavanju nove police za partnera s ID: {PartnerId}.", partnerId);
                throw;  //  bacanje iznimke prema višem sloju
            }
        }

        public bool CheckIsPolicyExisting( string policyNumber)
        {
            try
            {
                using (IDbConnection dbConnection = new SqlConnection(_connectionString))
                {
                    dbConnection.Open();

                    // SQL upit koji provjerava postoji li već polica sa zadanim brojem
                    var query = "SELECT COUNT(1) FROM PartnerPolica WHERE PolicyNumber = @PolicyNumber";

                    // Izvrši upit i provjeri je li broj polica veći od 0
                    var result = dbConnection.ExecuteScalar<int>(query, new { PolicyNumber = policyNumber });

                    // Ako COUNT(1) vraća više od 0, znači da polica postoji
                    return result > 0;
                }
            }
            catch (SqlException ex)
            {
                // Logiranje SQL greške
                _logger.LogError(ex, "SQL pogreška prilikom provjere postojanja police s brojem: {PolicyNumber}.", policyNumber);
                throw;  //  bacanje iznimke prema višem sloju
            }
            catch (Exception ex)
            {
                // Logiranje drugih grešaka
                _logger.LogError(ex, "Došlo je do pogreške pri provjeri postojanja police s brojem: {PolicyNumber}.", policyNumber);
                throw;  //  bacanje iznimke prema višem sloju
            }
        }

        public bool IsExternalCodeUnique(string externalCode)
        {
            try
            {
                using (IDbConnection dbConnection = new SqlConnection(_connectionString))
                {
                    dbConnection.Open();
                    var query = "SELECT COUNT(1) FROM Partner WHERE ExternalCode = @ExternalCode";
                    var result = dbConnection.ExecuteScalar<int>(query, new { ExternalCode = externalCode });
                    // Ako COUNT(1) vraća više od 0, znači da ExternalCode postoji
                    return result > 0;
                }
            }
            catch (SqlException ex)
            {
                // Logiranje SQL greške
                _logger.LogError(ex, "SQL pogreška prilikom provjere jedinstvenosti ExternalCode: {ExternalCode}.", externalCode);
                throw;  // bacanje iznimke prema višem sloju
            }
            catch (Exception ex)
            {
                // Logiranje drugih grešaka
                _logger.LogError(ex, "Došlo je do pogreške pri provjeri jedinstvenosti ExternalCode: {ExternalCode}.", externalCode);
                throw;  // bacanje iznimke prema višem sloju
            }
        }
      
    }
}
