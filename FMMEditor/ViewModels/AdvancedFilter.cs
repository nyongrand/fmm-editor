using ReactiveUI;
using System.Reactive.Concurrency;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FMMEditor.ViewModels
{
    public static class FilterOptions
    {
        public static readonly IReadOnlyList<string> OperatorOptions = ["AND", "OR"];
        public static readonly IReadOnlyList<string> StringComparisonOptions = ["contains", "is"];
        public static readonly IReadOnlyList<string> NumberComparisonOptions = ["at least", "at most", "is"];
    }

    public enum FilterFieldKind
    {
        String,
        Number
    }

    public record FilterField<TModel>(string Key, string DisplayName, FilterFieldKind Kind, Func<TModel, object?> Selector);

    public class FilterCondition : ReactiveObject
    {
        private string propertyKey = string.Empty;
        private string comparison = string.Empty;
        private string value = string.Empty;
        private FilterFieldKind fieldKind;
        private string logicalOperator = "AND";

        public string PropertyKey
        {
            get => propertyKey;
            set => this.RaiseAndSetIfChanged(ref propertyKey, value);
        }

        public string Comparison
        {
            get => comparison;
            set => this.RaiseAndSetIfChanged(ref comparison, value);
        }

        public string Value
        {
            get => value;
            set => this.RaiseAndSetIfChanged(ref this.value, value);
        }

        public FilterFieldKind FieldKind
        {
            get => fieldKind;
            set
            {
                this.RaiseAndSetIfChanged(ref fieldKind, value);
                this.RaisePropertyChanged(nameof(AvailableComparisons));
            }
        }

        public string LogicalOperator
        {
            get => logicalOperator;
            set => this.RaiseAndSetIfChanged(ref logicalOperator, value);
        }

        public IEnumerable<string> AvailableComparisons => FieldKind == FilterFieldKind.String
            ? FilterOptions.StringComparisonOptions
            : FilterOptions.NumberComparisonOptions;
    }

    public class AdvancedFilter<TModel>
    {
        private readonly Dictionary<string, FilterField<TModel>> fieldLookup;
        private readonly Action refreshAction;

        public IReadOnlyList<string> OperatorOptions => FilterOptions.OperatorOptions;
        public ObservableCollection<FilterCondition> Conditions { get; } = [];
        public IReadOnlyList<FilterField<TModel>> Fields { get; }

        public ReactiveCommand<Unit, Unit> AddConditionCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearConditionsCommand { get; }
        public ReactiveCommand<Unit, Unit> ApplyFilterCommand { get; }
        public ReactiveCommand<FilterCondition, Unit> RemoveConditionCommand { get; }

        public AdvancedFilter(IEnumerable<FilterField<TModel>> fields, Action refreshAction, IScheduler? scheduler = null)
        {
            Fields = [.. fields];
            fieldLookup = Fields.ToDictionary(f => f.Key);
            this.refreshAction = refreshAction;

            var outputScheduler = scheduler ?? RxApp.MainThreadScheduler;

            AddConditionCommand = ReactiveCommand.Create(AddConditionImpl, outputScheduler: outputScheduler);
            ClearConditionsCommand = ReactiveCommand.Create(ClearConditionsImpl, outputScheduler: outputScheduler);
            ApplyFilterCommand = ReactiveCommand.Create(() => refreshAction(), outputScheduler: outputScheduler);
            RemoveConditionCommand = ReactiveCommand.Create<FilterCondition>(RemoveConditionImpl, outputScheduler: outputScheduler);
        }

        public bool Matches(TModel model)
        {
            if (Conditions.Count == 0) return true;

            bool? result = null;
            foreach (var condition in Conditions)
            {
                var conditionResult = EvaluateCondition(model, condition);

                if (result == null)
                {
                    result = conditionResult;
                }
                else
                {
                    if (string.Equals(condition.LogicalOperator, "OR", StringComparison.OrdinalIgnoreCase))
                    {
                        result = result.Value || conditionResult;
                    }
                    else
                    {
                        result = result.Value && conditionResult;
                    }
                }
            }

            return result ?? true;
        }

        private bool EvaluateCondition(TModel model, FilterCondition condition)
        {
            if (!fieldLookup.TryGetValue(condition.PropertyKey, out var field)) return true;

            var value = field.Selector(model);

            if (condition.FieldKind == FilterFieldKind.String)
            {
                var target = condition.Value ?? string.Empty;
                if (string.IsNullOrWhiteSpace(target)) return true;

                var text = value?.ToString() ?? string.Empty;
                return condition.Comparison switch
                {
                    "contains" => text.Contains(target, StringComparison.OrdinalIgnoreCase),
                    "is" => string.Equals(text, target, StringComparison.OrdinalIgnoreCase),
                    _ => true
                };
            }

            if (string.IsNullOrWhiteSpace(condition.Value)) return true;
            if (!double.TryParse(condition.Value, out var targetNumber)) return false;

            var numericValue = ToDouble(value);
            if (numericValue == null) return false;

            return condition.Comparison switch
            {
                "at least" => numericValue.Value >= targetNumber,
                "at most" => numericValue.Value <= targetNumber,
                "is" => Math.Abs(numericValue.Value - targetNumber) < double.Epsilon,
                _ => true
            };
        }

        private static double? ToDouble(object? value)
        {
            return value switch
            {
                byte b => b,
                short s => s,
                int i => i,
                long l => l,
                float f => f,
                double d => d,
                decimal m => (double)m,
                _ => null
            };
        }

        private void AddConditionImpl()
        {
            var field = Fields[0];
            var condition = new FilterCondition
            {
                PropertyKey = field.Key,
                FieldKind = field.Kind,
                Comparison = GetDefaultComparison(field.Kind),
                Value = string.Empty,
                LogicalOperator = "AND"
            };

            AttachConditionHandler(condition);
            Conditions.Add(condition);
        }

        private void RemoveConditionImpl(FilterCondition condition)
        {
            Conditions.Remove(condition);
            refreshAction();
        }

        private void ClearConditionsImpl()
        {
            Conditions.Clear();
            refreshAction();
        }

        private void AttachConditionHandler(FilterCondition condition)
        {
            condition.WhenAnyValue(c => c.PropertyKey)
                .Subscribe(_ => UpdateConditionFieldKind(condition));
        }

        private void UpdateConditionFieldKind(FilterCondition condition)
        {
            if (!fieldLookup.TryGetValue(condition.PropertyKey, out var field)) return;

            condition.FieldKind = field.Kind;
            var comparisons = field.Kind == FilterFieldKind.String
                ? FilterOptions.StringComparisonOptions
                : FilterOptions.NumberComparisonOptions;
            if (!comparisons.Contains(condition.Comparison))
            {
                condition.Comparison = comparisons[0];
            }
        }

        private static string GetDefaultComparison(FilterFieldKind kind)
        {
            return kind == FilterFieldKind.String ? FilterOptions.StringComparisonOptions[0] : FilterOptions.NumberComparisonOptions[0];
        }
    }
}
