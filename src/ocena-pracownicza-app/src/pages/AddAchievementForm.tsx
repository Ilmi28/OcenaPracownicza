import React, { useEffect, useState, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import axiosClient from "../services/axiosClient";

export type AchievementModel = {
    name: string;
    description: string;
    date: string;
    employeeId: string;
    category: string;                      
    evaluationPeriodId: string;
    finalScore: string;
    achievementsSummary: string;
    isDraft: boolean;
};

interface DictionaryItem {
    elementGuid: string;
    code: string;
    name: string;
    unit: string;
    area: string;
    basePoints: number;
    isStackable: boolean;
}

interface EvaluationPeriodBasicInfo { 
    id: string; 
    name: string; 
    isClosed?: boolean; 
}

interface ApiCategoryItem {
    id: number;
    name: string;
}

interface CurrentUser { id: string; firstName: string; lastName: string; }
type ApiError = { response?: { data?: { message?: string; }; }; message?: string; };

const formatToLocal = (iso: string) => {
    if (!iso) return "";
    return iso.slice(0, 16);
};

const getErrorMessage = (error: unknown, fallback: string) => {
    const apiError = error as ApiError;
    return apiError.response?.data?.message || apiError.message || fallback;
};

type Props = { initialEmployeeId?: string; onSuccess?: () => void; };

const AddAchievementForm: React.FC<Props> = ({ initialEmployeeId = "", onSuccess }) => {
    const navigate = useNavigate();
    
    const [currentUser, setCurrentUser] = useState<CurrentUser | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    
    const [detectedPeriod, setDetectedPeriod] = useState<string | null>("Pobieranie...");
    const [isPeriodClosed, setIsPeriodClosed] = useState<boolean>(false);

    const [isConfirmationOpen, setIsConfirmationOpen] = useState(false);
    const [isCompletenessConfirmed, setIsCompletenessConfirmed] = useState(false);
    const [message, setMessage] = useState<{ type: "success" | "error"; text: string; } | null>(null);

    const [dictionaryItems, setDictionaryItems] = useState<DictionaryItem[]>([]);
    const [isLoadingDictionary, setIsLoadingDictionary] = useState(true);
    
    const [apiCategories, setApiCategories] = useState<ApiCategoryItem[]>([]);
    
    const [selectedArea, setSelectedArea] = useState<string>("Scientific");

    const [form, setForm] = useState<AchievementModel>({
        name: "",
        description: "",
        date: (() => {
            const local = new Date();
            return new Date(local.getTime() - local.getTimezoneOffset() * 60000).toISOString();
        })(),
        employeeId: initialEmployeeId,
        category: "0", 
        evaluationPeriodId: "",
        finalScore: "",
        achievementsSummary: "",
        isDraft: false,
    });

    const [file, setFile] = useState<File | null>(null);

    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const response = await axiosClient.get<ApiCategoryItem[]>("/achievement/categories");
                setApiCategories(response.data);
                if (response.data.length > 0) {
                    setForm(prev => ({ ...prev, category: response.data[0].id.toString() }));
                }
            } catch (error) {
                console.error("Nie udało się pobrać kategorii osiągnięć.", error);
            }
        };
        fetchCategories();
    }, []);

    const fetchDictionary = async () => {
        if (!form.evaluationPeriodId) return;
        setIsLoadingDictionary(true);
        try {
            const response = await axiosClient.get("/achievementelement");
            
            const itemsArray: DictionaryItem[] = response.data
                .filter((el: any) => el.evaluationPeriodId === form.evaluationPeriodId)
                .map((val: any) => ({
                    elementGuid: val.id,                
                    code: val.code,
                    name: val.name,
                    unit: val.departmentName || "Inne",                
                    area: val.activity === 0 ? "Scientific" : "Didactic",          
                    basePoints: val.basePoints,
                    isStackable: val.isStackable 
                }));

            setDictionaryItems(itemsArray);
        } catch (error) {
            setMessage({ type: "error", text: "Nie udało się pobrać szablonów osiągnięć dla tego okresu." });
        } finally {
            setIsLoadingDictionary(false);
        }
    };

    useEffect(() => {
        fetchDictionary();
    }, [form.evaluationPeriodId]);

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

    const [selectedElementId, setSelectedElementId] = useState<string>("");

    useEffect(() => {
        if (availableItems.length > 0) {
            const exists = availableItems.some(i => i.elementGuid === selectedElementId);
            if (!exists) {
                setSelectedElementId(availableItems[0].elementGuid);
            }
        } else {
            setSelectedElementId("");
        }
    }, [selectedArea, availableItems]);

    const currentDictionaryItem = useMemo(() => {
        return dictionaryItems.find(i => i.elementGuid === selectedElementId);
    }, [selectedElementId, dictionaryItems]);

    useEffect(() => {
        if (currentDictionaryItem) {
            setForm(prev => ({ 
                ...prev, 
                name: currentDictionaryItem.name,
                finalScore: currentDictionaryItem.basePoints.toString()
            }));
        }
    }, [currentDictionaryItem]);

    useEffect(() => {
        const fetchCurrentUserInfo = async () => {
            try {
                const resp = await axiosClient.get("/employee/me");
                const userData = resp.data?.data || resp.data;
                
                if (userData) {
                    setCurrentUser(userData);
                    if (userData.id) {
                        setForm(prev => ({ ...prev, employeeId: userData.id }));
                    }
                }
            } catch (error) {
                setMessage({ type: "error", text: "Nie udało się pobrać danych profilu." });
            }
        };
        fetchCurrentUserInfo();
    }, []);

    useEffect(() => {
        const fetchPeriod = async () => {
            try {
                const resp = await axiosClient.get<EvaluationPeriodBasicInfo>(`/evaluation-periods/by-date?date=${form.date}`);
                
                const allPeriodsResp = await axiosClient.get("/evaluation-periods");
                const currentPeriodFromList = allPeriodsResp.data.find((p: any) => p.id === resp.data.id);

                const isClosed = currentPeriodFromList?.isClosed ?? false;

                setDetectedPeriod(resp.data.name);
                setIsPeriodClosed(isClosed);                   
                setForm(prev => ({ ...prev, evaluationPeriodId: resp.data.id }));

                if (isClosed) {
                    setMessage({ type: "error", text: `Okres "${resp.data.name}" jest nieaktywny. Nie można dodawać nowych osiągnięć.` });
                } else {
                    setMessage(null);
                }

            } catch {
                setDetectedPeriod(null);
                setIsPeriodClosed(false);
                setForm(prev => ({ ...prev, evaluationPeriodId: "" }));
                setSelectedElementId("");
                setDictionaryItems([]);
            }
        };
        if (form.date) fetchPeriod();
    }, [form.date]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        
        setForm(prev => {
            if (name === "date") {
                const localDate = new Date(value);
                const isoString = isNaN(localDate.getTime())
                    ? new Date().toISOString()
                    : new Date(localDate.getTime() - localDate.getTimezoneOffset() * 60000).toISOString().slice(0, -1);
                
                return { ...prev, [name]: isoString };
            }
            return { ...prev, [name]: value };
        });
    };

    const isFormValid = useMemo(() => {
        return (
            Boolean(form.name) &&
            Boolean(form.description.trim()) &&
            Boolean(form.date) &&
            Boolean(form.employeeId) &&
            Boolean(form.evaluationPeriodId) &&
            Boolean(selectedElementId) &&
            Boolean(form.category) &&
            Boolean(String(form.finalScore).trim()) &&
            Boolean(detectedPeriod) &&
            !isPeriodClosed                               
        );
    }, [form, selectedElementId, detectedPeriod, isPeriodClosed]);

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
        if (!detectedPeriod || !currentDictionaryItem || isPeriodClosed) {
            setMessage({ type: "error", text: "Nie można zapisać: Wybrany okres ewaluacyjny jest zamknięty (nieaktywny)." });
            return;
        }

        setIsSubmitting(true);
        setMessage(null);

        try {
            const formData = new FormData();
            formData.append("Name", form.name);
            formData.append("Description", form.description);
            formData.append("Date", form.date);
            formData.append("EmployeeId", form.employeeId);
            formData.append("Category", form.category); 
            formData.append("AchievementElementId", currentDictionaryItem.elementGuid);
            formData.append("EvaluationPeriodId", form.evaluationPeriodId);
            formData.append("FinalScore", form.finalScore);
            formData.append("AchievementsSummary", form.description.slice(0, 100));
            formData.append("IsDraft", String(isDraft));

            if (file) {
                formData.append("File", file);
            }

            await axiosClient.post("/achievement", formData, {
                headers: { "Content-Type": "multipart/form-data" }
            });

            if (onSuccess) onSuccess();
            navigate("/achievements");

        } catch (error) {
            setMessage({
                type: "error",
                text: getErrorMessage(error, "Błąd zapisu.")
            });
        } finally {
            setFile(null);
            setIsSubmitting(false);
        }
    };

    return (
        <form className="achievement-form">
            <div className="form-header">
                <button type="button" onClick={() => navigate(-1)} className="back-btn">← Wróć</button>
                <h2>Dodaj Osiągnięcie</h2>
                {currentUser && (
                    <span style={{ marginLeft: 'auto', fontSize: '0.85rem', color: '#64748b' }}>
                        Zgłaszający: {currentUser.firstName} {currentUser.lastName}
                    </span>
                )}
            </div>

            {message && <div className={`alert ${message.type}`}>{message.text}</div>}

            <div className="form-group">
                <label htmlFor="date">Data zaistnienia osiągnięcia</label>
                <input id="date" name="date" type="datetime-local" value={formatToLocal(form.date)} onChange={handleChange} required />
                
                <div style={{ 
                    marginTop: '8px', 
                    fontSize: '0.85rem', 
                    padding: '10px', 
                    borderRadius: '6px', 
                    backgroundColor: isPeriodClosed ? '#e2e8f0' : (detectedPeriod && detectedPeriod !== "Pobieranie..." ? '#f0fff4' : '#fff5f5'), 
                    color: isPeriodClosed ? '#475569' : (detectedPeriod && detectedPeriod !== "Pobieranie..." ? '#276749' : '#c53030'), 
                    border: `1px solid ${isPeriodClosed ? '#cbd5e1' : (detectedPeriod && detectedPeriod !== "Pobieranie..." ? '#c6f6d5' : '#feb2b2')}`, 
                    fontWeight: '600' 
                }}>
                    {detectedPeriod === "Pobieranie..." ? (
                        <span>Sprawdzanie okresu...</span>
                    ) : isPeriodClosed ? (
                        <span>Okres: {detectedPeriod} (NIEAKTYWNY - brak możliwości rejestracji wniosków)</span>
                    ) : detectedPeriod ? (
                        <span>{detectedPeriod} (Aktywny)</span>
                    ) : (
                        <span>Nieznany okres - zmień datę!</span>
                    )}
                </div>
            </div>

            {isLoadingDictionary ? (
                <div className="loading-placeholder">Pobieranie bazy kryteriów osiągnięć dla wybranego okresu...</div>
            ) : (
                <>
                    <div className="form-group">
                        <label>Obszar Oceny</label>
                        <select value={selectedArea} onChange={(e) => setSelectedArea(e.target.value)} style={{ fontWeight: 'bold' }} disabled={isPeriodClosed}>
                            <option value="Scientific">Działalność Naukowo-Badawcza</option>
                            <option value="Didactic">Działalność Dydaktyczno-Organizacyjna</option>
                        </select>
                    </div>

                    <div className="form-group">
                        <label htmlFor="elementSelect">Osiągnięcie</label>
                        <select 
                            id="elementSelect" 
                            value={selectedElementId} 
                            onChange={(e) => setSelectedElementId(e.target.value)} 
                            disabled={isPeriodClosed}
                        >
                            {dictionaryItems.length === 0 ? (
                                <option value="">Brak szablonów dla tego okresu</option>
                            ) : Object.entries(groupedItems).map(([unit, items]) => (
                                <optgroup key={unit} label={`Dział: ${unit}`}>
                                    {items.map((item) => (
                                        <option key={item.elementGuid} value={item.elementGuid}>
                                            [{item.code}] {item.name}
                                        </option>
                                    ))}
                                </optgroup>
                            ))}
                        </select>
                    </div>
                </>
            )}

            <div className="form-group">
                <label htmlFor="category">Kategoria Biznesowa</label>
                <select id="category" name="category" value={form.category} onChange={handleChange} disabled={isPeriodClosed}>
                    {apiCategories.map((cat) => (
                        <option key={cat.id} value={cat.id}>
                            {cat.name}
                        </option>
                    ))}
                </select>
            </div>

            <hr style={{ borderTop: '1px dashed #cbd5e1', margin: '10px 0' }} />

            <div style={{ display: 'flex', gap: '1rem', flexWrap: 'wrap' }}>
                <div className="form-group" style={{ width: '100%' }}>
                    <label htmlFor="finalScore">Wynik punktowy</label>
                    <input
                        id="finalScore"
                        name="finalScore"
                        type="number"
                        value={form.finalScore}
                        onChange={handleChange}
                        required
                        disabled={isPeriodClosed}
                        style={{
                            backgroundColor: isPeriodClosed ? '#e2e8f0' : '#fff',
                            color: '#0f172a',
                            fontWeight: 'bold'
                        }}
                    />
                    <small style={{ color: '#4e73df', fontSize: '0.75rem', fontWeight: '600' }}>
                        Przełożony zweryfikuje liczbę punktów. Sugerowana: {currentDictionaryItem?.basePoints || 0}.
                        {currentDictionaryItem?.isStackable && " (Można wielokrotnie dodawać to osiągnięcie)."}
                    </small>
                </div>
            </div>

            <div className="form-group">
                <label htmlFor="description">Opis i uwagi</label>
                <textarea id="description" name="description" value={form.description} onChange={handleChange} required rows={3} disabled={isPeriodClosed} />
            </div>
            <div className="form-group">
                <label>Załącznik (opcjonalnie)</label>
                <input
                    type="file"
                    onChange={(e) => setFile(e.target.files?.[0] || null)}
                    disabled={isPeriodClosed}
                />
            </div>
            
            <div className="button-group" style={{ display: 'flex', gap: '1rem', marginTop: '1rem' }}>
                <button type="button" className="submit-btn" onClick={() => submitForm(true)} disabled={isSubmitting || !isFormValid} style={{ background: '#718096' }}>
                    Zapisywanie jako szkic
                </button>
                <button type="button" className="submit-btn" onClick={handleFinalSubmit} disabled={isSubmitting || !isFormValid}>
                    Prześlij do weryfikacji
                </button>
            </div>

            {isConfirmationOpen && (
                <div className="confirmation-overlay" role="dialog" aria-modal="true">
                    <div className="confirmation-card">
                        <h3>Potwierdź wysłanie zgłoszenia</h3>
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
                                Wyślij
                            </button>
                        </div>
                    </div>
                </div>
            )}

            <style>{`
                .achievement-form { width: 100%; max-width: 600px; margin: 2rem auto; display: grid; gap: 1.25rem; padding: 2.5rem; background: #ffffff; border-radius: 12px; box-shadow: 0 10px 25px rgba(0,0,0,0.05); font-family: 'Inter', sans-serif; box-sizing: border-box; }
                .form-header { display: flex; align-items: center; gap: 1rem; margin-bottom: 0.5rem; }
                .back-btn { background: none; border: none; color: #4e73df; cursor: pointer; font-weight: 600; }
                .form-group { display: flex; flex-direction: column; gap: 0.4rem; min-width: 0; }
                label { font-size: 0.85rem; font-weight: 700; color: #4e5d78; }
                input, textarea, select { width: 100%; max-width: 100%; box-sizing: border-box; padding: 12px; border: 1px solid #dce1e8; border-radius: 8px; background-color: #f9fbff; color: #2e3b4e; font-size: 0.95rem; }
                input:disabled, textarea:disabled, select:disabled { background-color: #e2e8f0; color: #64748b; cursor: not-allowed; }
                select { text-overflow: ellipsis; white-space: nowrap; overflow: hidden; }
                optgroup { font-weight: 700; color: #475569; background: #f8fafc; }
                option { font-weight: normal; color: #0f172a; background: #fff; padding: 4px; }
                .submit-btn { padding: 14px; cursor: pointer; background: #4e73df; color: white; border: none; border-radius: 8px; font-weight: 700; font-size: 1rem; width: 100%; box-sizing: border-box; }
                .secondary-btn { padding: 14px; cursor: pointer; background: #edf2f7; color: #2d3748; border: 1px solid #dce1e8; border-radius: 8px; font-weight: 700; font-size: 1rem; box-sizing: border-box;}
                .submit-btn:disabled, .secondary-btn:disabled { cursor: not-allowed; opacity: 0.4; background: #cbd5e1 !important; color: #94a3b8; }
                .alert.error { background: #fed7d7; color: #822727; padding: 10px; border-radius: 5px; box-sizing: border-box;}
                .confirmation-overlay { position: fixed; inset: 0; background: rgba(15, 23, 42, 0.45); display: flex; align-items: center; justify-content: center; padding: 1.5rem; z-index: 1000; }
                .confirmation-card { width: min(100%, 640px); max-height: 90vh; overflow-y: auto; background: #ffffff; border-radius: 16px; padding: 1.5rem; box-shadow: 0 24px 48px rgba(15, 23, 42, 0.2); display: grid; gap: 1rem; box-sizing: border-box; }
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