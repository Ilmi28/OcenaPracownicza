import React, { useEffect, useState, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import axiosClient from "../services/axiosClient";

export interface AchievementListItem {
    id: string;
    name: string;
    description: string;
    date: string;
    category: number;
    evaluationPeriodName: string;
    finalScore: string;
    achievementsSummary: string;
    stage2Status: number; 
    stage2Comment?: string | null;                
    employeeId: string;
    createdAt: string;
    evaluationPeriodId?: string;
    achievementElementId?: string;
}

interface EvaluationPeriodBasicInfo {
    id: string;
    name: string;
    isClosed: boolean;
}

interface UserInfo {
    userId: string;
    userName: string;
    email: string;
    role: string;
    roles: string[];
}

const KATEGORIE: Record<number, { nazwa: string; kolor: string }> = {
    0: { nazwa: "Sukces projektowy", kolor: "#007bff" },
    1: { nazwa: "Rozwój techniczny", kolor: "#9176c2" },
    2: { nazwa: "Ulepszenie procesu", kolor: "#6f42c1" },
    3: { nazwa: "Mentoring", kolor: "#28a745" },
    4: { nazwa: "Innowacja", kolor: "#fd7e14" },
    5: { nazwa: "Liderowanie", kolor: "#6c757d" },
    6: { nazwa: "Sukces klienta", kolor: "#2a567c" },
};

const STATUS_BADGES: Record<number, { tekst: string; klasa: string }> = {
    0: { tekst: "SZKIC", klasa: "badge-draft" },
    1: { tekst: "OCZEKUJĄCE", klasa: "badge-pending" },
    2: { tekst: "ZAAKCEPTOWANE", klasa: "badge-approved" },
    3: { tekst: "ODRZUCONE", klasa: "badge-rejected" },
    4: { tekst: "ZAMKNIĘTE", klasa: "badge-closed" },
    5: { tekst: "ZARCHIWIZOWANE", klasa: "badge-archived" },
};

const AchievementList: React.FC = () => {
    const [achievements, setAchievements] = useState<AchievementListItem[]>([]);
    const [periods, setPeriods] = useState<EvaluationPeriodBasicInfo[]>([]);
    const [selectedPeriodId, setSelectedPeriodId] = useState<string>("all");
    const [loading, setLoading] = useState(true);
    const [isEmployee, setIsEmployee] = useState(false);                
    const navigate = useNavigate();

    const [isSubmitModalOpen, setIsSubmitModalOpen] = useState(false);
    const [isEditModalOpen, setIsEditModalOpen] = useState(false);
    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false); 
    const [isCompletenessConfirmed, setIsCompletenessConfirmed] = useState(false);
    const [selectedItem, setSelectedItem] = useState<AchievementListItem | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const [editForm, setEditForm] = useState({
        name: "",
        description: "",
        finalScore: ""
    });

    const fetchData = async () => {
        setLoading(true);
        try {
            const userResp = await axiosClient.get<UserInfo>("/auth/me");
            setIsEmployee(userResp.data.role === "Employee");

            const periodsResp = await axiosClient.get<EvaluationPeriodBasicInfo[]>("/evaluation-periods");
            setPeriods(periodsResp.data);

            if (periodsResp.data.length > 0 && selectedPeriodId === "all") {
                const activePeriod = periodsResp.data.find(p => !p.isClosed);
                if (activePeriod) {
                    setSelectedPeriodId(activePeriod.id);
                } else {
                    setSelectedPeriodId(periodsResp.data[0].id);
                }
            }

            const achievementsResp = await axiosClient.get<AchievementListItem[]>("/achievement");
            setAchievements(achievementsResp.data);
        } catch (err: any) {
            console.error("Błąd pobierania danych.", err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    const filteredAndSortedAchievements = useMemo(() => {
        let result = [...achievements];

        if (selectedPeriodId !== "all") {
            result = result.filter(item => item.evaluationPeriodId === selectedPeriodId);
        }

        return result.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());
    }, [achievements, selectedPeriodId]);

    const openSubmitModal = (item: AchievementListItem) => {
        setSelectedItem(item);
        setIsCompletenessConfirmed(false);
        setIsSubmitModalOpen(true);
    };

    const openEditModal = (item: AchievementListItem) => {
        setSelectedItem(item);
        setEditForm({
            name: item.name,
            description: item.description,
            finalScore: item.finalScore
        });
        setIsEditModalOpen(true);
    };

    const openDeleteModal = (item: AchievementListItem) => {
        setSelectedItem(item);
        setIsDeleteModalOpen(true);
    };

    const saveAchievement = async (isDraft: boolean, customData?: typeof editForm) => {
        if (!selectedItem) return;
        setIsSubmitting(true);

        try {
            const formData = new FormData();
            formData.append("Name", customData ? customData.name : selectedItem.name);
            formData.append("Description", customData ? customData.description : selectedItem.description);
            formData.append("Date", selectedItem.date);
            formData.append("EmployeeId", selectedItem.employeeId);
            formData.append("Category", selectedItem.category.toString()); 
            formData.append("AchievementElementId", selectedItem.achievementElementId || "");
            formData.append("EvaluationPeriodId", selectedItem.evaluationPeriodId || "");
            formData.append("FinalScore", customData ? customData.finalScore : selectedItem.finalScore);
            formData.append("AchievementsSummary", (customData ? customData.description : selectedItem.description).slice(0, 100));
            formData.append("IsDraft", String(isDraft));

            await axiosClient.put(`/achievement/${selectedItem.id}`, formData, {
                headers: { "Content-Type": "multipart/form-data" }
            });

            setIsSubmitModalOpen(false);
            setIsEditModalOpen(false);
            setSelectedItem(null);
            fetchData();
        } catch (error) {
            console.error("Błąd podczas aktualizacji osiągnięcia:", error);
            alert("Wystąpił błąd podczas modyfikacji danych.");
        } finally {
            setIsSubmitting(false);
        }
    };

    const handleDeleteAchievement = async () => {
        if (!selectedItem) return;
        setIsSubmitting(true);

        try {
            await axiosClient.delete(`/achievement/${selectedItem.id}`);
            setIsDeleteModalOpen(false);
            setSelectedItem(null);
            fetchData();
        } catch (error) {
            console.error("Błąd podczas usuwania osiągnięcia:", error);
            alert("Nie udało się usunąć szkicu.");
        } finally {
            setIsSubmitting(false);
        }
    };

    const getRowClass = (status: number): string => {
        switch (status) {
            case 0: return "row-draft";       
            case 1: return "row-pending";     
            case 2: return "row-approved";    
            case 3: return "row-rejected";    
            case 4: return "row-closed";      
            case 5: return "row-archived";    
            default: return "";
        }
    };

    if (loading) return <div className="loader-table">Ładowanie danych...</div>;

    return (
        <div className="table-container">
            <header className="table-header">
                <div>
                    <h1>Lista Osiągnięć</h1>
                    <span className="count-tag">
                        {filteredAndSortedAchievements.length} pozycji
                    </span>
                </div>
                
                <div className="period-filter-container">
                    <label htmlFor="period-select">Okres oceniania:</label>
                    <select 
                        id="period-select"
                        value={selectedPeriodId} 
                        onChange={(e) => setSelectedPeriodId(e.target.value)}
                        className="select-period-filter"
                    >
                        <option value="all">Wszystkie okresy</option>
                        {periods.map(p => (
                            <option key={p.id} value={p.id}>
                                {p.name} {p.isClosed ? "(Nieaktywny)" : "(Aktywny)"}
                            </option>
                        ))}
                    </select>
                </div>

                <div className="table-actions">
                    <button onClick={fetchData} className="btn-refresh">
                        Odśwież
                    </button>
                    <button
                        onClick={() => navigate("/achievement/add")}
                        className="btn-add-primary"
                    >
                        + Dodaj osiągnięcie
                    </button>
                </div>
            </header>

            <div className="responsive-table-wrapper">
                <table className="achievement-table">
                    <thead>
                        <tr>
                            <th className="th-date">Data</th>
                            <th className="th-cat">Kategoria</th>
                            <th className="th-period">Okres</th>
                            <th className="th-score">Wynik</th>
                            <th className="th-name">Nazwa</th>
                            <th className="th-desc">Opis</th>
                            <th className="th-comment">Uwagi przełożonego</th> {         }
                            {!isEmployee && <th className="th-emp">Pracownik</th>}
                            <th className="th-actions" style={{ textAlign: "right" }}>Akcje</th>
                        </tr>
                    </thead>
                    <tbody>
                        {filteredAndSortedAchievements.map((item) => {
                            const kat = KATEGORIE[item.category] || KATEGORIE[5];
                            const isDraft = item.stage2Status === 0;
                            const shouldStrike = item.stage2Status === 4 || item.stage2Status === 5;
                            const badgeInfo = STATUS_BADGES[item.stage2Status];
                            
                            const showCommentStatus = [2, 3, 4, 5].includes(item.stage2Status);

                            return (
                                <tr key={item.id} className={getRowClass(item.stage2Status)}>
                                    <td className="col-date">
                                        {new Date(item.date).toLocaleDateString("pl-PL")}
                                    </td>
                                    <td>
                                        <span
                                            className="cat-badge"
                                            style={{
                                                borderColor: kat.kolor,
                                                color: kat.kolor,
                                            }}
                                        >
                                            {kat.nazwa}
                                        </span>
                                    </td>
                                    <td className="col-period">
                                        <span className="period-tag">
                                            {item.evaluationPeriodName}
                                        </span>
                                    </td>
                                    <td>{item.finalScore}</td>
                                    <td className={`col-name ${shouldStrike ? "text-strike" : ""}`}>
                                        <div className="name-wrapper">
                                            <div>
                                                {badgeInfo && (
                                                    <span className={`status-badge-base ${badgeInfo.klasa}`}>
                                                        {badgeInfo.tekst}
                                                    </span>
                                                )}
                                                {item.name}
                                            </div>
                                        </div>
                                    </td>
                                    <td>
                                        <div className="col-desc" title={item.description}>
                                            {item.description}
                                        </div>
                                    </td>
                                    {               }
                                    <td className="col-comment">
                                        {showCommentStatus && item.stage2Comment && item.stage2Comment.trim() !== "" ? (
                                            <div>
                                                <strong>Uwaga:</strong> {item.stage2Comment}
                                            </div>
                                        ) : (
                                            <span className="text-muted">-</span>
                                        )}
                                    </td>
                                    {!isEmployee && (
                                        <td className="col-emp">
                                            <code>{item.employeeId.slice(0, 8)}</code>
                                        </td>
                                    )}
                                    <td style={{ textAlign: "right" }}>
                                        {isDraft && (
                                            <div className="action-buttons-container">
                                                <button
                                                    className="btn-edit"
                                                    onClick={() => openEditModal(item)}
                                                >
                                                    Edytuj
                                                </button>
                                                <button
                                                    className="btn-delete"
                                                    onClick={() => openDeleteModal(item)}
                                                >
                                                    Usuń
                                                </button>
                                                <button
                                                    className="btn-edit"
                                                    onClick={() => openSubmitModal(item)}
                                                >
                                                    Wyślij do oceny
                                                </button>
                                            </div>
                                        )}
                                    </td>
                                </tr>
                            );
                        })}
                        {filteredAndSortedAchievements.length === 0 && (
                            <tr>
                                <td colSpan={isEmployee ? 8 : 9} style={{ textAlign: "center", color: "#718096", padding: "40px" }}>
                                    Brak zarejestrowanych osiągnięć w wybranym okresie.
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            {                     }
            {isSubmitModalOpen && selectedItem && (
                <div className="confirmation-overlay" role="dialog" aria-modal="true">
                    <div className="confirmation-card">
                        <h3>Potwierdź wysłanie zgłoszenia</h3>
                        <dl className="confirmation-summary">
                            <div>
                                <dt>Osiągnięcie</dt>
                                <dd>{selectedItem.name}</dd>
                            </div>
                            <div>
                                <dt>Opis i uwagi</dt>
                                <dd style={{ whiteSpace: "pre-wrap" }}>{selectedItem.description}</dd>
                            </div>
                            <div>
                                <dt>Okres ewaluacyjny</dt>
                                <dd>{selectedItem.evaluationPeriodName}</dd>
                            </div>
                            <div>
                                <dt>Punkty</dt>
                                <dd>{selectedItem.finalScore}</dd>
                            </div>
                        </dl>
                        <label className="confirmation-checkbox">
                            <input 
                                type="checkbox" 
                                checked={isCompletenessConfirmed} 
                                onChange={(e) => setIsCompletenessConfirmed(e.target.checked)} 
                            />
                            <span>Potwierdzam, że zgłoszenie jest kompletne i gotowe do oceny.</span>
                        </label>
                        <div className="confirmation-actions">
                            <button 
                                type="button" 
                                className="secondary-btn" 
                                onClick={() => setIsSubmitModalOpen(false)} 
                                disabled={isSubmitting}
                            >
                                Wróć
                            </button>
                            <button 
                                type="button" 
                                className="submit-btn" 
                                onClick={() => saveAchievement(false)} 
                                disabled={!isCompletenessConfirmed || isSubmitting}
                            >
                                {isSubmitting ? "Wysyłanie..." : "Wyślij do weryfikacji"}
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {                  }
            {isEditModalOpen && selectedItem && (
                <div className="confirmation-overlay" role="dialog" aria-modal="true">
                    <div className="confirmation-card">
                        <h3>Edytuj Szkic Osiągnięcia</h3>
                        <div className="modal-form-group">
                            <label>Nazwa osiągnięcia</label>
                            <input 
                                type="text" 
                                value={editForm.name} 
                                onChange={(e) => setEditForm({...editForm, name: e.target.value})} 
                            />
                        </div>
                        <div className="modal-form-group">
                            <label>Wynik punktowy</label>
                            <input 
                                type="number" 
                                value={editForm.finalScore} 
                                onChange={(e) => setEditForm({...editForm, finalScore: e.target.value})} 
                            />
                        </div>
                        <div className="modal-form-group">
                            <label>Opis i uwagi</label>
                            <textarea 
                                rows={4} 
                                value={editForm.description} 
                                onChange={(e) => setEditForm({...editForm, description: e.target.value})} 
                            />
                        </div>
                        <div className="confirmation-actions" style={{ marginTop: "1rem" }}>
                            <button 
                                type="button" 
                                className="secondary-btn" 
                                onClick={() => setIsEditModalOpen(false)} 
                                disabled={isSubmitting}
                            >
                                Anuluj
                            </button>
                            <button 
                                type="button" 
                                className="submit-btn" 
                                style={{ background: "#718096" }}
                                onClick={() => saveAchievement(true, editForm)} 
                                disabled={isSubmitting || !editForm.name || !editForm.description}
                            >
                                {isSubmitting ? "Zapisywanie..." : "Zapisz jako szkic"}
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {                  }
            {isDeleteModalOpen && selectedItem && (
                <div className="confirmation-overlay" role="dialog" aria-modal="true">
                    <div className="confirmation-card">
                        <h3>Usuń szkic osiągnięcia</h3>
                        <div style={{ padding: "0.5rem 0", color: "#1e293b", fontSize: "0.95rem" }}>
                            Czy na pewno chcesz bezpowrotnie usunąć szkic wniosku: <strong>{selectedItem.name}</strong>? Operacji tej nie da się cofnąć.
                        </div>
                        <div className="confirmation-actions" style={{ marginTop: "0.5rem" }}>
                            <button 
                                type="button" 
                                className="secondary-btn" 
                                onClick={() => setIsDeleteModalOpen(false)} 
                                disabled={isSubmitting}
                            >
                                Anuluj
                            </button>
                            <button 
                                type="button" 
                                className="submit-btn" 
                                style={{ background: "#dc2626" }} 
                                onClick={handleDeleteAchievement} 
                                disabled={isSubmitting}
                            >
                                {isSubmitting ? "Usuwanie..." : "Potwierdź usunięcie"}
                            </button>
                        </div>
                    </div>
                </div>
            )}

            <style>{`
                .table-container {
                    padding: 2rem;
                    background-color: #ffffff;
                    min-height: 100vh;
                    font-family: 'Inter', sans-serif;
                }

                .table-header {
                    display: flex;
                    justify-content: space-between;
                    align-items: center;
                    margin-bottom: 2rem;
                    gap: 15px;
                    flex-wrap: wrap;
                }

                .table-header h1 { font-size: 1.6rem; margin: 0; color: #1a202c; }
                .count-tag { font-size: 0.85rem; color: #718096; margin-left: 10px; font-weight: 500; }

                .period-filter-container {
                    display: flex;
                    align-items: center;
                    gap: 10px;
                    margin-left: auto;
                }
                .period-filter-container label {
                    font-size: 0.9rem;
                    font-weight: 600;
                    color: #4a5568;
                }
                .select-period-filter {
                    padding: 8px 14px;
                    border: 1px solid #cbd5e1;
                    border-radius: 8px;
                    background-color: #f8fafc;
                    color: #1e293b;
                    font-size: 0.9rem;
                    font-weight: 600;
                    cursor: pointer;
                    outline: none;
                    transition: 0.15s ease;
                }
                .select-period-filter:focus {
                    border-color: #4e73df;
                    box-shadow: 0 0 0 3px rgba(78, 115, 223, 0.15);
                }

                .table-actions { display: flex; gap: 12px; }

                .btn-refresh { background: #edf2f7; border: none; padding: 10px 18px; border-radius: 8px; cursor: pointer; color: #4a5568; font-weight: 600; }
                .btn-add-primary { background: #4e73df; border: none; padding: 10px 20px; border-radius: 8px; cursor: pointer; color: white; font-weight: 600; transition: 0.2s; }
                .btn-add-primary:hover { background: #2e59d9; }

                .responsive-table-wrapper {
                    border: 1px solid #e2e8f0;
                    border-radius: 12px;
                    overflow-x: auto;
                    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
                    background: white;
                }

                .achievement-table {
                    width: 100%;
                    border-collapse: collapse;
                    text-align: left;
                    font-size: 0.95rem;
                    table-layout: fixed;
                }

                .achievement-table thead {
                    background-color: #f8fafc;
                    border-bottom: 2px solid #e2e8f0;
                }

                .achievement-table th {
                    padding: 16px;
                    color: #4a5568;
                    font-weight: 700;
                    text-transform: uppercase;
                    font-size: 0.75rem;
                    letter-spacing: 0.05em;
                }

                .th-date { width: 120px; }
                .th-cat { width: 140px; }
                .th-period { width: 130px; }
                .th-score { width: 50px; }
                .th-name { width: 260px; }
                .th-desc { width: 220px; }
                .th-comment { width: 220px; } /* Szerokość nowej kolumny */
                .th-emp { width: 100px; }
                .th-actions { width: 280px; }

                .achievement-table td {
                    padding: 16px;
                    border-bottom: 1px solid #edf2f7;
                    color: #2d3748;
                    vertical-align: top;
                    word-wrap: break-word;
                    overflow-wrap: break-word;
                }

                .achievement-table tbody tr:hover {
                    background-color: #f9fbff;
                }

                .row-draft { background-color: #f8fafc !important; }
                .row-draft:hover { background-color: #f1f5f9 !important; }

                .row-pending { background-color: #fffbeb !important; }
                .row-pending:hover { background-color: #fef3c7 !important; }

                .row-approved { background-color: #f0fdf4 !important; }
                .row-approved:hover { background-color: #dcfce7 !important; }

                .row-rejected { background-color: #fff5f5 !important; }
                .row-rejected:hover { background-color: #fed7d7 !important; }

                .row-closed, .row-archived { background-color: #ffffff !important; color: #000000 !important; }
                .row-closed td, .row-archived td { color: #000000 !important; }
                .row-closed:hover, .row-archived:hover { background-color: #f8fafc !important; }

                .text-strike { text-decoration: line-through; }

                .status-badge-base {
                    padding: 3px 8px;
                    border-radius: 4px;
                    font-size: 0.7rem;
                    font-weight: 700;
                    margin-right: 8px;
                    display: inline-block;
                    vertical-align: middle;
                }

                .badge-draft { background-color: #e2e8f0; color: #475569; }
                .badge-pending { background-color: #ecc94b; color: #744210; }
                .badge-approved { background-color: #48bb78; color: #ffffff; }
                .badge-rejected { background-color: #f56565; color: #ffffff; }
                .badge-closed { background-color: #1a202c; color: #ffffff; }
                .badge-archived { background-color: #4a5568; color: #ffffff; }

                .name-wrapper {
                    display: flex;
                    flex-direction: column;
                    gap: 6px;
                }

                /* Dostosowany styl chmurki komentarza w osobnej kolumnie */
                .manager-comment-bubble {
                    background: #f8fafc;
                    border-left: 3px solid #cbd5e1;
                    padding: 8px 12px;
                    border-radius: 4px;
                    font-size: 0.85rem;
                    color: #475569;
                    font-weight: 500;
                    line-height: 1.4;
                }

                .row-rejected .manager-comment-bubble {
                    background: #fff5f5;
                    border-left-color: #f56565;
                    color: #9b2c2c;
                }
                
                .row-approved .manager-comment-bubble {
                    background: #f0fdf4;
                    border-left-color: #48bb78;
                    color: #14532d;
                }

                .cat-badge {
                    padding: 4px 10px;
                    border: 1px solid;
                    border-radius: 6px;
                    font-size: 0.75rem;
                    font-weight: 700;
                    background: white;
                    display: inline-block;
                    white-space: nowrap;
                }

                .col-date { font-weight: 500; color: #718096; }
                .col-name { font-weight: 600; white-space: normal; }
                .col-comment { vertical-align: middle; }

                .col-desc { 
                    display: -webkit-box;
                    -webkit-line-clamp: 3;
                    -webkit-box-orient: vertical;
                    overflow: hidden;
                    text-overflow: ellipsis;
                    white-space: normal;
                    color: #718096;
                    font-size: 0.9rem;
                    line-height: 1.5;
                }

                .text-muted {
                    color: #cbd5e1;
                    font-weight: bold;
                }

                .col-emp code { background: #f1f5f9; padding: 2px 6px; border-radius: 4px; font-size: 0.85rem; }

                .action-buttons-container { display: flex; gap: 6px; justify-content: flex-end; }

                .btn-edit {
                    background: white;
                    border: 1px solid #cbd5e1;
                    padding: 6px 12px;
                    border-radius: 6px;
                    cursor: pointer;
                    font-size: 0.85rem;
                    color: #475569;
                    font-weight: 600;
                    transition: 0.2s;
                }
                .btn-edit:hover { background: #f1f5f9; }

                .btn-delete {
                    background: white;
                    border: 1px solid #fca5a5;
                    padding: 6px 12px;
                    border-radius: 6px;
                    cursor: pointer;
                    font-size: 0.85rem;
                    color: #dc2626;
                    font-weight: 600;
                    transition: 0.2s;
                }
                .btn-delete:hover { background: #fef2f2; }

                .btn-submit {
                    background: #ecc94b;
                    border: 1px solid #d69e2e;
                    padding: 6px 12px;
                    border-radius: 6px;
                    cursor: pointer;
                    font-size: 0.85rem;
                    color: #744210;
                    font-weight: 600;
                    transition: 0.2s;
                }
                .btn-submit:hover { background: #d69e2e; color: white; }

                .confirmation-overlay { position: fixed; inset: 0; background: rgba(15, 23, 42, 0.45); display: flex; align-items: center; justify-content: center; padding: 1.5rem; z-index: 1000; }
                .confirmation-card { width: min(100%, 550px); max-height: 90vh; overflow-y: auto; background: #ffffff; border-radius: 16px; padding: 1.5rem; box-shadow: 0 24px 48px rgba(15, 23, 42, 0.2); display: grid; gap: 1rem; box-sizing: border-box; text-align: left; }
                .confirmation-summary div { padding: 0.75rem; border-radius: 10px; background: #f8fafc; margin-bottom: 5px; }
                .confirmation-summary dt { font-size: 0.8rem; font-weight: 700; color: #4a5568; }
                .confirmation-summary dd { margin: 0; color: #1f2937; font-weight: 600; }
                .confirmation-checkbox { display: flex; gap: 0.75rem; font-weight: 500; align-items: center; font-size: 0.9rem; color: #4a5568; cursor: pointer; }
                .confirmation-actions { display: flex; justify-content: flex-end; gap: 0.75rem; }
                
                .modal-form-group { display: flex; flex-direction: column; gap: 0.4rem; }
                .modal-form-group label { font-size: 0.85rem; font-weight: 700; color: #4e5d78; }
                .modal-form-group input, .modal-form-group textarea { width: 100%; box-sizing: border-box; padding: 10px; border: 1px solid #dce1e8; border-radius: 8px; background-color: #f9fbff; color: #2e3b4e; font-size: 0.95rem; }

                .submit-btn { padding: 10px 20px; cursor: pointer; background: #4e73df; color: white; border: none; border-radius: 8px; font-weight: 700; font-size: 0.95rem; }
                .secondary-btn { padding: 10px 20px; cursor: pointer; background: #edf2f7; color: #2d3748; border: 1px solid #dce1e8; border-radius: 8px; font-weight: 700; font-size: 0.95rem; }
                .submit-btn:disabled, .secondary-btn:disabled { cursor: not-allowed; opacity: 0.4; background: #cbd5e1 !important; color: #94a3b8; }

                .loader-table { padding: 100px; text-align: center; font-size: 1.2rem; color: #a0aec0; }
            `}</style>
        </div>
    );
};

export default AchievementList;