import React, { useState, useEffect, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import axiosClient from "../services/axiosClient";

export type AchievementModel = {
    name: string;
    description: string;
    date: string;
    employeeId: string;
    category: number;
    evaluationPeriodId: string;
    finalScore: string;
    achievementsSummary: string;
    isDraft: boolean;
};

type ScoreType = "Fixed" | "Multiplied" | "Variable" | "Formula";

interface DictionaryItem {
    id: number;
    code: string;
    name: string;
    unit: string;
    area: string;
    basePoints: number;
    scoreType: ScoreType;
}

interface CurrentUser { id: string; firstName: string; lastName: string; }
interface ApiResponse<T> { success: boolean; message: string; data: T; }
interface EvaluationPeriodBasicInfo { id: string; name: string; }
type ApiError = { response?: { data?: { message?: string; }; }; message?: string; };

const POLISH_LOCALE = "pl-PL";
const formatToLocal = (iso: string) => iso.slice(0, 16);

const getErrorMessage = (error: unknown, fallback: string) => {
    const apiError = error as ApiError;
    return apiError.response?.data?.message || apiError.message || fallback;
};

type Props = { initialEmployeeId?: string; onSuccess?: () => void; };

const AddAchievementForm: React.FC<Props> = ({ initialEmployeeId = "", onSuccess }) => {
    const navigate = useNavigate();
    
    const [currentUser, setCurrentUser] = useState<CurrentUser | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [loadingUser, setLoadingUser] = useState(true);
    const [detectedPeriod, setDetectedPeriod] = useState<string | null>("Pobieranie...");
    const [isConfirmationOpen, setIsConfirmationOpen] = useState(false);
    const [isCompletenessConfirmed, setIsCompletenessConfirmed] = useState(false);
    const [message, setMessage] = useState<{ type: "success" | "error"; text: string; } | null>(null);

    const [dictionaryItems, setDictionaryItems] = useState<DictionaryItem[]>([]);
    const [isLoadingDictionary, setIsLoadingDictionary] = useState(true);
    
    const [selectedArea, setSelectedArea] = useState<string>("Scientific");
    const [multiplier, setMultiplier] = useState<number>(1);

    const [form, setForm] = useState<AchievementModel>({
        name: "",
        description: "",
        date: new Date().toISOString(),
        employeeId: initialEmployeeId,
        category: 0, 
        evaluationPeriodId: "",
        finalScore: "",
        achievementsSummary: "",
        isDraft: false,
    });

    useEffect(() => {
        const fetchDictionary = async () => {
            try {
                const response = await axiosClient.get("/achievement/dictionary");
                
                const itemsArray: DictionaryItem[] = Object.entries(response.data).map(([key, val]: [string, any]) => ({
                    id: Number(key),
                    code: val.code,
                    name: val.name,
                    unit: val.unit,
                    area: val.area,
                    basePoints: val.basePoints,
                    scoreType: val.scoreType
                }));

                setDictionaryItems(itemsArray);
            } catch (error) {
                setMessage({ type: "error", text: "Nie udało się pobrać słownika osiągnięć z serwera." });
            } finally {
                setIsLoadingDictionary(false);
            }
        };

        fetchDictionary();
    }, []);

    const availableItems = useMemo(() => {
        return dictionaryItems.filter(item => item.area === selectedArea);
    }, [selectedArea, dictionaryItems]);

    const groupedItems = useMemo(() => {
        return availableItems.reduce((acc, item) => {
            if (!acc[item.unit]) acc[item.unit] = [];
            acc[item.unit].push(item);
            return acc;
        }, {} as Record<string, DictionaryItem[]>);
    }, [availableItems]);

    useEffect(() => {
        if (availableItems.length > 0 && !availableItems.find(i => i.id === form.category)) {
            setForm(prev => ({ ...prev, category: availableItems[0].id }));
            setMultiplier(1);
        }
    }, [selectedArea, availableItems, form.category]);

    const currentDictionaryItem = useMemo(() => {
        return dictionaryItems.find(i => i.id === form.category);
    }, [form.category, dictionaryItems]);

    // Automatycznie ustawiamy form.name na podstawie wybranego osiągnięcia ze słownika
    useEffect(() => {
        if (currentDictionaryItem) {
            setForm(prev => ({ ...prev, name: currentDictionaryItem.name }));
        }
    }, [currentDictionaryItem]);

    useEffect(() => {
        if (currentDictionaryItem) {
            if (currentDictionaryItem.scoreType === "Fixed") {
                setForm(prev => ({ ...prev, finalScore: currentDictionaryItem.basePoints.toString() }));
            } else if (currentDictionaryItem.scoreType === "Multiplied") {
                setForm(prev => ({ ...prev, finalScore: (currentDictionaryItem.basePoints * multiplier).toString() }));
            } else {
                if (form.finalScore === "") {
                    setForm(prev => ({ ...prev, finalScore: currentDictionaryItem.basePoints.toString() }));
                }
            }
        }
    }, [currentDictionaryItem, multiplier]);

    useEffect(() => {
        const fetchCurrentUserInfo = async () => {
            try {
                const resp = await axiosClient.get<ApiResponse<CurrentUser>>("/employee/me");
                setCurrentUser(resp.data.data);
                setForm(prev => ({ ...prev, employeeId: resp.data.data.id }));
            } catch (error) {
                setMessage({ type: "error", text: "Nie udało się pobrać danych profilu." });
            } finally {
                setLoadingUser(false);
            }
        };
        fetchCurrentUserInfo();
    }, []);

    useEffect(() => {
        const fetchPeriod = async () => {
            try {
                const resp = await axiosClient.get<EvaluationPeriodBasicInfo>(`/evaluation-periods/by-date?date=${form.date}`);
                setDetectedPeriod(resp.data.name);
                setForm(prev => ({ ...prev, evaluationPeriodId: resp.data.id }));
            } catch {
                setDetectedPeriod(null);
                setForm(prev => ({ ...prev, evaluationPeriodId: "" }));
            }
        };
        if (form.date) fetchPeriod();
    }, [form.date]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setForm(prev => ({
            ...prev,
            [name]: name === "category" ? Number(value) : name === "date" ? new Date(value).toISOString() : value,
        }));
    };

    const isFormValid = useMemo(() => {
        return (
            Boolean(form.name) &&
            Boolean(form.description.trim()) &&
            Boolean(form.date) &&
            Boolean(form.employeeId) &&
            Boolean(form.evaluationPeriodId) &&
            Boolean(String(form.finalScore).trim()) &&
            Boolean(detectedPeriod)
        );
    }, [form, detectedPeriod]);

    const handleFinalSubmit = () => {
        if (!isFormValid) {
            setMessage({
                type: "error",
                text: "Uzupełnij wszystkie wymagane pola przed wysłaniem zgłoszenia.",
            });
            return;
        }

        setIsCompletenessConfirmed(false);
        setIsConfirmationOpen(true);
        setMessage(null);
    };

    const submitForm = async (isDraft: boolean) => {
        if (!detectedPeriod) {
            setMessage({ type: "error", text: "Nie można zapisać: Wybrana data nie pasuje do żadnego okresu." });
            return;
        }

        setIsSubmitting(true);
        setMessage(null);
        try {
            await axiosClient.post("/achievement", { ...form, isDraft });
            if (onSuccess) onSuccess();
            navigate("/achievements");
        } catch (error) {
            setMessage({ type: "error", text: getErrorMessage(error, "Błąd zapisu.") });
            setIsSubmitting(false);
        }
    };

    return (
        <form className="achievement-form">
            <div className="form-header">
                <button type="button" onClick={() => navigate(-1)} className="back-btn">← Wróć</button>
                <h2>Dodaj Osiągnięcie</h2>
            </div>

            {message && <div className={`alert ${message.type}`}>{message.text}</div>}

            {isLoadingDictionary ? (
                <div className="loading-placeholder">Pobieranie słownika osiągnięć...</div>
            ) : (
                <>
                    <div className="form-group">
                        <label>Obszar Oceny</label>
                        <select value={selectedArea} onChange={(e) => setSelectedArea(e.target.value)} style={{ fontWeight: 'bold' }}>
                            <option value="Scientific">Działalność Naukowo-Badawcza</option>
                            <option value="Didactic">Działalność Dydaktyczno-Organizacyjna</option>
                        </select>
                    </div>

                    <div className="form-group">
                        <label htmlFor="category">Klasyfikacja Osiągnięcia</label>
                        <select id="category" name="category" value={form.category} onChange={handleChange}>
                            {Object.entries(groupedItems).map(([unit, items]) => (
                                <optgroup key={unit} label={`🏫 ${unit}`}>
                                    {items.map((item) => (
                                        <option key={item.id} value={item.id}>
                                            [{item.code}] {item.name}
                                        </option>
                                    ))}
                                </optgroup>
                            ))}
                        </select>
                    </div>
                </>
            )}

            <hr style={{ borderTop: '1px dashed #cbd5e1', margin: '10px 0' }} />

            <div style={{ display: 'flex', gap: '1rem', flexWrap: 'wrap' }}>
                {currentDictionaryItem?.scoreType === "Multiplied" && (
                    <div className="form-group" style={{ flex: 1 }}>
                        <label>Liczba (np. studentów, prac, miesięcy)</label>
                        <input
                            type="number"
                            min="1"
                            value={multiplier}
                            onChange={(e) => setMultiplier(Math.max(1, Number(e.target.value)))}
                        />
                    </div>
                )}
                
                <div className="form-group" style={{ flex: currentDictionaryItem?.scoreType === "Multiplied" ? 1 : 'none', width: currentDictionaryItem?.scoreType !== "Multiplied" ? '100%' : 'auto' }}>
                    <label htmlFor="finalScore">Wynik punktowy</label>
                    <input
                        id="finalScore"
                        name="finalScore"
                        type="number"
                        value={form.finalScore}
                        onChange={handleChange}
                        required
                        disabled={currentDictionaryItem?.scoreType === "Fixed" || currentDictionaryItem?.scoreType === "Multiplied"}
                        style={{
                            backgroundColor: (currentDictionaryItem?.scoreType === "Fixed" || currentDictionaryItem?.scoreType === "Multiplied") ? '#f1f5f9' : '#fff',
                            color: '#0f172a',
                            fontWeight: 'bold'
                        }}
                    />
                    <small style={{ color: '#64748b', fontSize: '0.75rem' }}>
                        {currentDictionaryItem?.scoreType === "Fixed" && "Punkty stałe, wyliczone automatycznie."}
                        {currentDictionaryItem?.scoreType === "Multiplied" && `Naliczone: ${multiplier} × ${currentDictionaryItem.basePoints} pkt`}
                        {currentDictionaryItem?.scoreType === "Variable" && `Podaj wartość ręcznie (Sugerowane: ${currentDictionaryItem.basePoints} pkt)`}
                        {currentDictionaryItem?.scoreType === "Formula" && `Ostateczna wartość wyliczana przez dział ewaluacji.`}
                    </small>
                </div>
            </div>

            <div className="form-group">
                <label htmlFor="date">Data zaistnienia osiągnięcia</label>
                <input id="date" name="date" type="datetime-local" value={formatToLocal(form.date)} onChange={handleChange} required />
                <div style={{ marginTop: '8px', fontSize: '0.85rem', padding: '10px', borderRadius: '6px', backgroundColor: detectedPeriod ? '#f0fff4' : '#fff5f5', color: detectedPeriod ? '#276749' : '#c53030', border: `1px solid ${detectedPeriod ? '#c6f6d5' : '#feb2b2'}`, fontWeight: '600' }}>
                    {detectedPeriod === "Pobieranie..." ? <span>🔍 Sprawdzanie okresu...</span> : detectedPeriod ? <span>Okres: {detectedPeriod}</span> : <span>⚠️ Nieznany okres - zmień datę!</span>}
                </div>
            </div>

            <div className="form-group">
                <label htmlFor="description">Opis i uwagi</label>
                <textarea id="description" name="description" value={form.description} onChange={handleChange} required rows={3} />
            </div>

            <div className="button-group" style={{ display: 'flex', gap: '1rem', marginTop: '1rem' }}>
                <button type="button" className="submit-btn" onClick={() => submitForm(true)} disabled={isSubmitting || !detectedPeriod || isLoadingDictionary} style={{ background: '#718096' }}>
                    {isSubmitting ? "Zapisywanie..." : "Zapisz jako szkic"}
                </button>
                <button type="button" className="submit-btn" onClick={handleFinalSubmit} disabled={isSubmitting || !detectedPeriod || isLoadingDictionary}>
                    {isSubmitting ? "Zapisywanie..." : "Prześlij do weryfikacji"}
                </button>
            </div>

            {isConfirmationOpen && (
                <div className="confirmation-overlay" role="dialog" aria-modal="true">
                    <div className="confirmation-card">
                        <h3>Potwierdź wysłanie zgłoszenia</h3>
                        <p className="confirmation-copy">
                            Zgłoszenie zostanie wysłane do weryfikacji przez: <strong style={{ color: '#2563eb' }}>{currentDictionaryItem?.unit}</strong>
                        </p>
                        <dl className="confirmation-summary">
                            <div>
                                <dt>Obszar Oceny</dt>
                                <dd>{selectedArea === "Scientific" ? "Działalność Naukowo-Badawcza" : "Działalność Dydaktyczno-Organizacyjna"}</dd>
                            </div>
                            <div>
                                <dt>Osiągnięcie</dt>
                                <dd>[{currentDictionaryItem?.code}] {form.name}</dd>
                            </div>
                            <div>
                                <dt>Opis i uwagi</dt>
                                <dd style={{ whiteSpace: "pre-wrap" }}>{form.description}</dd>
                            </div>
                            <div>
                                <dt>Data zaistnienia</dt>
                                <dd>{form.date.replace('T', ' ').substring(0, 16)}</dd>
                            </div>
                            <div>
                                <dt>Okres ewaluacyjny</dt>
                                <dd>{detectedPeriod}</dd>
                            </div>
                            <div>
                                <dt>Punkty</dt>
                                <dd>{form.finalScore}</dd>
                        </div>
                        </dl>
                        <label className="confirmation-checkbox">
                            <input type="checkbox" checked={isCompletenessConfirmed} onChange={(e) => setIsCompletenessConfirmed(e.target.checked)} />
                            <span>Potwierdzam, że zgłoszenie jest kompletne.</span>
                        </label>
                        <div className="confirmation-actions">
                            <button type="button" className="secondary-btn" onClick={() => setIsConfirmationOpen(false)} disabled={isSubmitting}>Wróć</button>
                            <button type="button" className="submit-btn" onClick={() => { submitForm(false); setIsConfirmationOpen(false); }} disabled={!isCompletenessConfirmed || isSubmitting}>
                                {isSubmitting ? "Zapisywanie..." : "Wyślij"}
                            </button>
                        </div>
                    </div>
                </div>
            )}

            <style>{`
                .achievement-form { 
                    width: 100%;
                    max-width: 600px; 
                    margin: 2rem auto; 
                    display: grid; 
                    gap: 1.25rem; 
                    padding: 2.5rem; 
                    background: #ffffff; 
                    border-radius: 12px; 
                    box-shadow: 0 10px 25px rgba(0,0,0,0.05); 
                    font-family: 'Inter', sans-serif; 
                    box-sizing: border-box;
                }
                .form-header { display: flex; align-items: center; gap: 1rem; margin-bottom: 0.5rem; }
                .back-btn { background: none; border: none; color: #4e73df; cursor: pointer; font-weight: 600; }
                .form-group { display: flex; flex-direction: column; gap: 0.4rem; min-width: 0; }
                label { font-size: 0.85rem; font-weight: 700; color: #4e5d78; }
                input, textarea, select { 
                    width: 100%;
                    max-width: 100%;
                    box-sizing: border-box;
                    padding: 12px; 
                    border: 1px solid #dce1e8; 
                    border-radius: 8px; 
                    background-color: #f9fbff; 
                    color: #2e3b4e; 
                    font-size: 0.95rem; 
                }
                select {
                    text-overflow: ellipsis;
                    white-space: nowrap;
                    overflow: hidden;
                }
                optgroup { font-weight: 700; color: #475569; background: #f8fafc; }
                option { font-weight: normal; color: #0f172a; background: #fff; padding: 4px; }
                .submit-btn { padding: 14px; cursor: pointer; background: #4e73df; color: white; border: none; border-radius: 8px; font-weight: 700; font-size: 1rem; width: 100%; box-sizing: border-box; }
                .secondary-btn { padding: 14px; cursor: pointer; background: #edf2f7; color: #2d3748; border: 1px solid #dce1e8; border-radius: 8px; font-weight: 700; font-size: 1rem; box-sizing: border-box;}
                .submit-btn:disabled, .secondary-btn:disabled { cursor: not-allowed; opacity: 0.6; }
                .alert.error { background: #fed7d7; color: #822727; padding: 10px; border-radius: 5px; box-sizing: border-box;}
                .confirmation-overlay { position: fixed; inset: 0; background: rgba(15, 23, 42, 0.45); display: flex; align-items: center; justify-content: center; padding: 1.5rem; }
                .confirmation-card { width: min(100%, 640px); background: #ffffff; border-radius: 16px; padding: 1.5rem; box-shadow: 0 24px 48px rgba(15, 23, 42, 0.2); display: grid; gap: 1rem; box-sizing: border-box; }
                .confirmation-summary div { padding: 0.75rem; border-radius: 10px; background: #f8fafc; margin-bottom: 5px; }
                .confirmation-summary dt { font-size: 0.8rem; font-weight: 700; color: #4a5568; }
                .confirmation-summary dd { margin: 0; color: #1f2937; font-weight: 600; }
                .confirmation-checkbox { display: flex; gap: 0.75rem; font-weight: 500; }
                .confirmation-actions { display: flex; justify-content: flex-end; gap: 0.75rem; }
                .loading-placeholder { padding: 1rem; text-align: center; color: #4a5568; background: #f8fafc; border-radius: 8px; border: 1px dashed #cbd5e1; }
            `}</style>
        </form>
    );
};

export default AddAchievementForm;